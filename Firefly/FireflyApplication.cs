﻿using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Timers;
using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ShaderGen;
using System.Diagnostics;
using System.Collections.Generic;

namespace Firefly
{
    unsafe class FireflyApplication
    {
        public const uint Width = 512;
        public const uint Height = 512;
        public const uint ViewScale = 1;

        private Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private CommandList _commandList;
        private Texture _transferTex;
        private TextureView _texView;
        private RgbaFloat[] _buff;
        private ResourceSet _graphicsSet;
        private Pipeline _graphicsPipeline;

        private Color32 _color;
        private int _frameCount;

        public void Run()
        {
            _buff = new RgbaFloat[Width * Height];
            _color = new Color32(0, 0, 0);

            Renderer.InitRender(_color, _buff, RenderType.GouraudShading);
            Console.Write("加载场景:Scene1...");
            Renderer.LoadScene("Scene1");
            Console.WriteLine("完成");

            Console.Write("编译Shader...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> paths = new List<string>();
            foreach (KeyValuePair<string, Material> item in Renderer.Materials)
                paths.Add($"Shaders/{item.Value.ShaderName}.cs");
            ShaderGenerator.CompleShader(paths);
            stopwatch.Stop();
            Console.WriteLine($"完成，耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

            Renderer.DelegateCollections = ShaderGenerator.DelegateCollections;
            Renderer.ShaderInformation = ShaderGenerator.ShaderInformation;
            Renderer.InitMaterials();

            GraphicsBackend backend = VeldridStartup.GetPlatformDefaultBackend();

            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(100, 100, (int)(Width * ViewScale), (int)(Height * ViewScale), WindowState.Normal, "Firefly"),
                new GraphicsDeviceOptions(debug: false, swapchainDepthFormat: null, syncToVerticalBlank: false),
                backend,
                out _window,
                out _graphicsDevice);
            CreateDeviceResources();
            
            Timer timer1 = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timer1.Elapsed += Timer1_Elapsed;

            while (_window.Exists)
            {
                _window.PumpEvents();
                if (!_window.Exists) { break; }
                for (int i = 0; i < _buff.Length; i++)
                {
                    _buff[i] = new RgbaFloat();
                    Renderer.DepthBuff[i] = float.MaxValue;
                }
                RenderFrame();
                //System.Threading.Thread.Sleep(10);
            }

            _graphicsDevice.Dispose();

        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            _window.Title = "FPS:" + _frameCount;
            _frameCount = 0;
        }

        private void RenderFrame()
        {
            _frameCount++;
            _commandList.Begin();

            foreach (KeyValuePair<string, Entity> item in Renderer.CurrentScene.Entities)
                item.Value.Rotation += new Vector3(0.01f);
            Renderer.Draw();

            fixed (RgbaFloat* pixelDataPtr = _buff)
                _graphicsDevice.UpdateTexture(_transferTex, (IntPtr)pixelDataPtr, Width * Height * (uint)sizeof(RgbaFloat), 0, 0, 0, Width, Height, 1, 0, 0);

            _commandList.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
            _commandList.SetPipeline(_graphicsPipeline);
            _commandList.SetGraphicsResourceSet(0, _graphicsSet);
            _commandList.Draw(3);
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            _graphicsDevice.SwapBuffers();
        }

        private void CreateDeviceResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;
            _commandList = factory.CreateCommandList();
            _transferTex = factory.CreateTexture(
                TextureDescription.Texture2D(Width, Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.Sampled | TextureUsage.Storage));
            _texView = factory.CreateTextureView(_transferTex);

            ResourceLayout graphicsLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("SourceTex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _graphicsSet = factory.CreateResourceSet(new ResourceSetDescription(graphicsLayout, _texView, _graphicsDevice.LinearSampler));

            _graphicsPipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(
                    Array.Empty<VertexLayoutDescription>(),
                    new[]
                    {
                        factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, LoadShaderBytes("FramebufferBlitter-vertex"), "VS")),
                        factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, LoadShaderBytes("FramebufferBlitter-fragment"), "FS"))
                    }),
                graphicsLayout,
                _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription));
        }

        private byte[] LoadShaderBytes(string name)
        {
            string extension;
            switch (_graphicsDevice.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    extension = "hlsl.bytes";
                    break;
                case GraphicsBackend.Vulkan:
                    extension = "450.glsl.spv";
                    break;
                case GraphicsBackend.OpenGL:
                    extension = "330.glsl";
                    break;
                case GraphicsBackend.Metal:
                    extension = "metallib";
                    break;
                case GraphicsBackend.OpenGLES:
                    extension = "300.glsles";
                    break;
                default: throw new InvalidOperationException();
            }

            return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "BlitterShader", $"{name}.{extension}"));
        }
    }
}
