// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct FilmList : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static FilmList GetRootAsFilmList(ByteBuffer _bb) { return GetRootAsFilmList(_bb, new FilmList()); }
  public static FilmList GetRootAsFilmList(ByteBuffer _bb, FilmList obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public FilmList __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public Film? Films(int j) { int o = __p.__offset(4); return o != 0 ? (Film?)(new Film()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int FilmsLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<FilmList> CreateFilmList(FlatBufferBuilder builder,
      VectorOffset FilmsOffset = default(VectorOffset)) {
    builder.StartTable(1);
    FilmList.AddFilms(builder, FilmsOffset);
    return FilmList.EndFilmList(builder);
  }

  public static void StartFilmList(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddFilms(FlatBufferBuilder builder, VectorOffset FilmsOffset) { builder.AddOffset(0, FilmsOffset.Value, 0); }
  public static VectorOffset CreateFilmsVector(FlatBufferBuilder builder, Offset<Film>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateFilmsVectorBlock(FlatBufferBuilder builder, Offset<Film>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartFilmsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<FilmList> EndFilmList(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FilmList>(o);
  }
};

