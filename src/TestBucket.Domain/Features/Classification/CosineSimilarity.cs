using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Features.Classification;
public class CosineSimilarity
{
    public static double Calculate(float[] vector1, ReadOnlyMemory<float>? vector2)
    {
        if(vector2 is null)
        {
            return double.MaxValue;
        }
        return Calculate(vector1, vector2.Value.ToArray());
    }
    
    public static double Calculate(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
        {
            throw new ArgumentException("Vectors must be non-null and of equal length.");
        }

        double dotProduct = 0.0;
        double magnitude1 = 0.0;
        double magnitude2 = 0.0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += Math.Pow(vector1[i], 2);
            magnitude2 += Math.Pow(vector2[i], 2);
        }

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0.0; // Or handle the case where one or both vectors are zero vectors.
        }

        return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
    }
}