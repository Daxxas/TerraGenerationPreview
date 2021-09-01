
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

    private Expression e;

    public EquationHandler(string equation, Dictionary<string, EquationNoise> noiseList)
    {
        Argument argX = new Argument("x", 0);
        Argument argY = new Argument("y", 0);
        Argument argZ = new Argument("z", 0);
        
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

        e = new Expression(equation, elements);

    }

    public double EquationResult(float x, float y, float z)
    {
        e.setArgumentValue("x", x);
        e.setArgumentValue("y", y);
        e.setArgumentValue("z", z);
        
        double result = e.calculate();
        
        return result;
    }
}

public struct EquationNoise
{
    public int dimension;
    public FastNoiseLite noise;
}
