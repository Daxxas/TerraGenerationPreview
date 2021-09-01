
using System;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using Function = org.mariuszgromada.math.mxparser.Function;

public class EquationHandler
{
    private string currentEquation;

    private float currentX;
    private float currentY;
    private float currentZ;
    
    private Dictionary<string, EquationNoise> noiseList = new Dictionary<string, EquationNoise>();

    public Dictionary<string, EquationNoise> NoiseList
    {
        get => noiseList;
    }


    public void ChangeSeed(int seed)
    {
        foreach (var noise in noiseList)
        {
            noise.Value.noise.SetSeed(seed);
        }
    }


    public double EquationResult(string equation, int x, int y, int z)
    {

        Argument argX = new Argument("x", x);
        Argument argY = new Argument("y", y);
        Argument argZ = new Argument("z", z);
        
        PrimitiveElement[] elements = new PrimitiveElement[noiseList.Count + 3];

        elements[0] = argX;
        elements[1] = argY;
        elements[2] = argZ;
        
        int count = 3;
        foreach (var noiseElement in noiseList)
        {
            if (noiseElement.Value.dimension == 2)
            {
                org.mariuszgromada.math.mxparser.Function f = new org.mariuszgromada.math.mxparser.Function(noiseElement.Key, new FunctionNoise2D(noiseElement.Value.noise));
                elements[count] = f;
            }
            else if (noiseElement.Value.dimension == 3)
            {
                org.mariuszgromada.math.mxparser.Function f = new org.mariuszgromada.math.mxparser.Function(noiseElement.Key, new FunctionNoise3D(noiseElement.Value.noise));
                elements[count] = f;
            }

            count++;
        }
        
        Expression e = new Expression(equation, elements);
        
        double result = e.calculate();
        
        return result;
    }
}

public struct EquationNoise
{
    public int dimension;
    public FastNoiseLite noise;
}
