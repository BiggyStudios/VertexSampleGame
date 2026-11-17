using System.Numerics;

using Vertex;
using Vertex.Input;
using Vertex.Rendering;

// The engine keeps its state in statics (the active scene, input, time), so tests mustn't run at the same time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace SampleGame.Tests
{
    /// <summary>
    /// Loads the whole shooter and plays it for a while without a window: the check that a new engine version hasn't
    /// broken the game. Steam stays off, so it runs anywhere.
    /// </summary>
    public class SmokeTests : IDisposable
    {
        public SmokeTests()
        {
            Scene.Active = new Scene();
            Camera.Main = null;
            SimulatedInput.Reset();
            Time.FixedDeltaTime = 1f / 60f;
        }

        public void Dispose()
        {
            Scene.Active.Unload();
            Camera.Main = null;
            SimulatedInput.Reset();
        }

        [Fact]
        public void The_game_loads_and_plays()
        {
            var loop = new GameLoop(new global::SampleGame.SampleGame());
            loop.Load();

            // Found by name: until the first frame they're still waiting to join the scene, where only Find looks.
            GameObject multiplayer = Scene.Active.Find("Multiplayer")!;
            multiplayer.GetComponent<MultiplayerManager>()!.ConnectToSteamOnStart = false;
            multiplayer.GetComponent<MainMenu>()!.OpenOnStart = false;
            var player = (Player)Scene.Active.Find("Player")!;

            // Straight into practice: land on the ground first.
            RunFrames(loop, 60);
            Vector3 start = player.Transform.Position;

            // Then walk forward for two seconds, shooting.
            SimulatedInput.PressKey(Key.W);
            for (int i = 0; i < 120; i++)
            {
                if (i % 20 == 0) SimulatedInput.PressButton(MouseButton.Left);
                if (i % 20 == 10) SimulatedInput.ReleaseButton(MouseButton.Left);
                RunFrames(loop, 1);
            }
            SimulatedInput.ReleaseKey(Key.W);
            RunFrames(loop, 30);

            Vector3 end = player.Transform.Position;
            float walked = Vector2.Distance(new Vector2(start.X, start.Z), new Vector2(end.X, end.Z));

            Assert.False(player.IsDestroyed);
            Assert.True(walked > 2f, $"Holding W for two seconds should walk somewhere, but went {walked} m ({start} to {end}).");
            Assert.InRange(end.Y, -0.1f, 0.5f);
        }

        private static void RunFrames(GameLoop loop, int frames)
        {
            for (int i = 0; i < frames; i++) loop.Update(1.0 / 60.0, 1280, 720, 720);
        }
    }
}
