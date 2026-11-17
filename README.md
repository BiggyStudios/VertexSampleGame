# Vertex Sample Game

A Source-style multiplayer shooter (surf, grapples, rockets, bots, Steam lobbies) built on the
[Vertex engine](https://github.com/BiggyStudios/Vertex).

## Getting it

The engine comes in as a git submodule in the `Engine` folder, so clone with `--recursive`:

```
git clone --recursive https://github.com/BiggyStudios/VertexSampleGame.git
```

Already cloned without it? Run `git submodule update --init`.

## Running it

```
dotnet run --project SampleGame
```

It renders with Vulkan where it can and falls back to OpenGL; pass `-opengl` to choose OpenGL.

`dotnet test` plays the game headless for a few seconds, to check a new engine version hasn't broken it.

## Updating the engine

`Engine` is pinned to one engine commit. To move to a newer one:

```
cd Engine
git pull origin master
cd ..
git add Engine
git commit -m "Update the engine"
```

You can also change the engine right there in `Engine` (it's a full checkout of the Vertex repo): commit and push
those changes from inside `Engine`, then commit the new `Engine` pointer here.

## License

The game is under the Vertex Sample Game License (see [LICENSE](LICENSE)). The engine is under the MIT License.
