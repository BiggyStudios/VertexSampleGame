# Vertex Sample Game

A Source-style multiplayer shooter (surf, grapples, rockets, bots, Steam lobbies) built on the
[Vertex engine](https://github.com/BiggyStudios/Vertex).

## Getting it

The game builds against the engine it's cloned inside, so clone the engine first and the game into it:

```
git clone https://github.com/BiggyStudios/Vertex.git
cd Vertex
git clone https://github.com/BiggyStudios/VertexSampleGame.git
```

The engine repo ignores the `VertexSampleGame` folder, so the two stay separate repositories: commit and push each
from its own folder. Changes to the engine show up in the game straight away.

With the engine somewhere else, point the build at it: `dotnet run --project SampleGame -p:VertexDir=/path/to/Vertex/`
(the engine repo's folder). Folder names are case-sensitive on Linux. Opening the game in the Vertex editor doesn't need
either: the editor builds it against its own engine.

## Running it

```
cd VertexSampleGame
dotnet run --project SampleGame
```

It renders with Vulkan where it can and falls back to OpenGL; pass `-opengl` to choose OpenGL.

`dotnet test SampleGame.slnx` runs the engine's tests and then plays the game headless for a few seconds, to check
engine changes haven't broken it.

## License

The game is under the Vertex Sample Game License (see [LICENSE](LICENSE)). The engine is under the MIT License.
