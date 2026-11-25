using System.Numerics;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[GcServer(true)]
[MemoryDiagnoser]
public class SumFloat
{
    private List<float> floatList;
    private float[] floatArray;

    [Params(1460)] // Taille d'un cas réel : ~1,5 k valeurs
    public int N;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random(42);
        floatArray = new float[N];
        for (var i = 0; i < N; i++) floatArray[i] = (float)rand.NextDouble();
        floatList = new List<float>(floatArray);
    }

    [Benchmark(Baseline = true)]
    public float LinqSum()
    {
        // Référence : LINQ Sum()
        return floatList.Sum();
    }

    [Benchmark]
    public float ForLoop()
    {
        float res = 0f;
        var list = floatList;
        for (var i = 0; i < list.Count; i++)
        {
            res += list[i];
        }
        return res;
    }

    [Benchmark]
    public float ForeachLoop()
    {
        float res = 0f;
        foreach (var value in floatList)
        {
            res += value;
        }
        return res;
    }

    [Benchmark]
    public float SpanFor()
    {
        
        float res = 0f;
        var span = CollectionsMarshal.AsSpan(floatList);
        for (int i = 0; i < span.Length; i++)
        {
            res += span[i];
        }
        return res;
    }

    [Benchmark]
    public float VectorizedSIMD()
    {
        // Optimisation matérielle (si supportée par le CPU)
        var span = CollectionsMarshal.AsSpan(floatList);
        var vectorSize = Vector<float>.Count;
        var acc = new Vector<float>(0f);
        int i = 0;

        // On additionne par blocs (ex: 4 ou 8 floats à la fois)
        for (; i <= span.Length - vectorSize; i += vectorSize)
        {
            acc += new Vector<float>(span.Slice(i, vectorSize));
        }

        // Somme finale du vecteur
        float res = Vector.Dot(acc, Vector<float>.One);

        // Reste (si N n'est pas multiple de vectorSize)
        for (; i < span.Length; i++)
        {
            res += span[i];
        }
        return res;
    }
}