using System.Numerics;
using System.Runtime.CompilerServices;

using Vertex;
using Vertex.Rendering;

namespace SampleGame
{
    /// <summary>
    /// The maps' textures, made in code: tiles with a fine grain and a bevelled gap round each one, and ripples for water.
    /// Each comes with a normal map, so light catches the bevels and the grain as if they were really there.
    /// </summary>
    public static class Surfaces
    {
        private const int _size = 256;

        private static readonly ConditionalWeakTable<Texture, Texture> _normalMaps = new();
        private static Texture? _ripples;

        /// <summary>The normal map made with <paramref name="texture"/>, if it came from here.</summary>
        public static Texture? NormalMapFor(Texture? texture) =>
            texture != null && _normalMaps.TryGetValue(texture, out Texture? normalMap) ? normalMap : null;

        /// <summary>
        /// A checkerboard of tiles, two across and two down, in <paramref name="a"/> and <paramref name="b"/>: each a slab
        /// with a narrow, darker gap round it and a little grain, so speed and distance are easy to read but it doesn't
        /// look flat.
        /// </summary>
        public static Texture Tiles(Color a, Color b)
        {
            byte[] colors = new byte[_size * _size * 4];
            byte[] heights = new byte[_size * _size * 4];
            int half = _size / 2;

            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    bool first = (x / half + y / half) % 2 == 0;
                    Color color = first ? a : b;

                    // How far into the tile from its nearest edge, rising over a few pixels from the gap to its top.
                    int u = x % half, v = y % half;
                    int fromEdge = Math.Min(Math.Min(u, half - 1 - u), Math.Min(v, half - 1 - v));
                    float bevel = Math.Clamp((fromEdge - 1f) / 5f, 0f, 1f);

                    float grain = Noise(x, y, 32) * 0.5f + Noise(x, y, 8) * 0.3f + Noise(x, y, 2) * 0.2f;
                    float shade = (1f + (grain - 0.5f) * 0.14f) * float.Lerp(0.72f, 1f, bevel);

                    int i = (y * _size + x) * 4;
                    colors[i] = ToByte(color.R * shade);
                    colors[i + 1] = ToByte(color.G * shade);
                    colors[i + 2] = ToByte(color.B * shade);
                    colors[i + 3] = 255;
                    heights[i] = ToByte(bevel * 0.8f + grain * 0.2f);
                }
            }

            var texture = new Texture(_size, _size, colors);
            _normalMaps.Add(texture, Texture.CreateNormalMap(new Texture(_size, _size, heights), strength: 4f));
            return texture;
        }

        /// <summary>A normal map of small overlapping waves, for water. It tiles, so it can repeat across a whole sea.</summary>
        public static Texture Ripples => _ripples ??= MakeRipples();

        private static Texture MakeRipples()
        {
            byte[] heights = new byte[_size * _size * 4];
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    // Waves a whole number of times across, so the edges meet; noise breaks up their regularity.
                    float p = MathF.Tau / _size;
                    float wave = MathF.Sin((3f * x + y) * p) * 0.3f
                               + MathF.Sin((-2f * x + 5f * y) * p + 1.3f) * 0.22f
                               + MathF.Sin((7f * x - 3f * y) * p + 0.4f) * 0.12f
                               + (Noise(x, y, 16) - 0.5f) * 0.5f;
                    heights[(y * _size + x) * 4] = ToByte(0.5f + wave * 0.5f);
                }
            }

            return Texture.CreateNormalMap(new Texture(_size, _size, heights), strength: 3f);
        }

        /// <summary>Smooth noise from 0 to 1, varying over about <paramref name="cell"/> pixels, repeating across the texture.</summary>
        private static float Noise(int x, int y, int cell)
        {
            int cells = _size / cell;
            int cellX = x / cell, cellY = y / cell;
            float fx = Smooth((x % cell) / (float)cell);
            float fy = Smooth((y % cell) / (float)cell);

            float Corner(int cx, int cy) => Hash(((cx % cells) + cells) % cells, ((cy % cells) + cells) % cells, cell);
            float top = float.Lerp(Corner(cellX, cellY), Corner(cellX + 1, cellY), fx);
            float bottom = float.Lerp(Corner(cellX, cellY + 1), Corner(cellX + 1, cellY + 1), fx);
            return float.Lerp(top, bottom, fy);
        }

        private static float Smooth(float t) => t * t * (3f - 2f * t);

        private static float Hash(int x, int y, int seed)
        {
            uint h = (uint)x * 1597334673u ^ (uint)y * 3812015801u ^ (uint)seed * 2654435761u;
            h = (h ^ (h >> 16)) * 2246822519u;
            h ^= h >> 13;
            return h / (float)uint.MaxValue;
        }

        private static byte ToByte(float value) => (byte)MathF.Round(Math.Clamp(value, 0f, 1f) * 255f);
    }

    /// <summary>Keeps a water surface's ripples drifting, slowly, as if a breeze were moving them.</summary>
    public class WaterFlow : Component
    {
        public Vector2 Speed { get; set; } = new(0.012f, 0.007f);

        private Material? _material;

        protected override void Start()
        {
            _material = GameObject.GetComponent<MeshRenderer>()?.Material;
        }

        protected override void Update()
        {
            if (_material != null) _material.TextureOffset = Speed * Time.TotalTime;
        }
    }
}
