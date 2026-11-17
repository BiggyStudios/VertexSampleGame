# Vertex Sample Game

## Building

The game builds against the [Vertex engine](https://github.com/BiggyStudios/Vertex), so clone it into the engine's folder:

```
git clone https://github.com/BiggyStudios/Vertex.git
cd Vertex
git clone https://github.com/BiggyStudios/VertexSampleGame.git
cd VertexSampleGame
dotnet build SampleGame.slnx
```

## Running

```
dotnet run --project SampleGame
```

Add `-- -opengl` to run with OpenGL instead of Vulkan.

## Testing

```
dotnet test SampleGame.slnx
```
