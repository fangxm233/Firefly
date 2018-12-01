using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Timers;
using Firefly.Render;
using Firefly.Render.Renderable;
using Firefly.Render.Structure;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Firefly
{
    unsafe class FireflyApplication
    {
        public const uint Width = 512;
        public const uint Height = 512;
        public const uint ViewScale = 1;

        private Sdl2Window _window;
        private GraphicsDevice _gd;
        private CommandList _cl;
        private Texture _transferTex;
        private TextureView _texView;
        private RgbaFloat[] _fb;
        private ResourceSet _graphicsSet;
        private Pipeline _graphicsPipeline;

        private Color32 _color;
        private int FrameCount;

        public void Run()
        {
            GraphicsBackend backend = VeldridStartup.GetPlatformDefaultBackend();
            
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(100, 100, (int)(Width * ViewScale), (int)(Height * ViewScale), WindowState.Normal, "Firefly"),
                new GraphicsDeviceOptions(debug: false, swapchainDepthFormat: null, syncToVerticalBlank: false),
                backend,
                out _window,
                out _gd);
            CreateDeviceResources();

            _fb = new RgbaFloat[Width * Height];
            _color = new Color32(0, 0, 0);
            Renderer.StartRender(512, 512, _color, _fb, RenderType.GouraudShading);

            Renderer.Camera = new Camera();
            Renderer.Entities = new[]{
                new Entity(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Mesh(new []
                {
                    new Vertex(new Vector3(-0.5f , 0.5f , -0.5f ), Color.FromArgb(255, 82, 188)),
                    new Vertex(new Vector3(0.5f , 0.5f , -0.5f ), Color.FromArgb(82, 212, 255)),
                    new Vertex(new Vector3(-0.5f , -0.5f , -0.5f ), Color.FromArgb(82, 255, 94)),
                    new Vertex(new Vector3(0.5f , -0.5f , -0.5f ), Color.FromArgb(255, 237, 82)),
                    new Vertex(new Vector3(-0.5f , 0.5f , 0.5f ), Color.FromArgb(255, 237, 82)),
                    new Vertex(new Vector3(0.5f , 0.5f , 0.5f ), Color.FromArgb(82, 255, 94)),
                    new Vertex(new Vector3(-0.5f , -0.5f , 0.5f ), Color.FromArgb(82, 212, 255)),
                    new Vertex(new Vector3(0.5f , -0.5f , 0.5f ), Color.FromArgb(255, 82, 188))
                }, new int[]
                {
                    0, 1, 2,
                    3, 2, 1,
                    4, 6, 5,
                    7, 5, 6,
                    4, 0, 6,
                    2, 6, 0,
                    1, 5, 3,
                    7, 3, 5,
                    4, 5, 0,
                    1, 0, 5,
                    2, 3, 6,
                    7, 6, 3,
                }))
            };

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
                for (int i = 0; i < _fb.Length; i++)
                {
                    _fb[i] = new RgbaFloat();
                }
                RenderFrame();
            }

            _gd.Dispose();

        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            _window.Title = "FPS:" + FrameCount;
            FrameCount = 0;
        }

        private void RenderFrame()
        {
            FrameCount++;
            _cl.Begin();
            
            Renderer.Entities[0].Rotation += new Vector3(0.01f, 0.01f, 0.01f);
            Renderer.Draw();

            fixed (RgbaFloat* pixelDataPtr = _fb)
            {
                _gd.UpdateTexture(_transferTex, (IntPtr)pixelDataPtr, Width * Height * (uint)sizeof(RgbaFloat), 0, 0, 0, Width, Height, 1, 0, 0);
            }

            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.SetPipeline(_graphicsPipeline);
            _cl.SetGraphicsResourceSet(0, _graphicsSet);
            _cl.Draw(3);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers();
        }

        private void CreateDeviceResources()
        {
            ResourceFactory factory = _gd.ResourceFactory;
            _cl = factory.CreateCommandList();
            _transferTex = factory.CreateTexture(
                TextureDescription.Texture2D(Width, Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.Sampled | TextureUsage.Storage));
            _texView = factory.CreateTextureView(_transferTex);

            ResourceLayout graphicsLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("SourceTex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _graphicsSet = factory.CreateResourceSet(new ResourceSetDescription(graphicsLayout, _texView, _gd.LinearSampler));

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
                _gd.MainSwapchain.Framebuffer.OutputDescription));
        }

        private byte[] LoadShaderBytes(string name)
        {
            string extension;
            switch (_gd.BackendType)
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

            return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Shaders", $"{name}.{extension}"));
        }
    }
}
