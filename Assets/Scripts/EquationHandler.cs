
using System;
using System.Collections.Generic;
using NCalc;

public class EquationHandler
{
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
    

    public double ParseEquation(string equation, int x, int y, int z)
    {
        Expression e = new Expression(equation);
        
        e.EvaluateParameter += (s, args) =>
        {
            if (s == "x")
            {
                args.Result = (float) x;
            }
            if (s == "y")
            {
                args.Result = (float) y;
            }
            if (s == "z")
            {
                args.Result = (float) z;
            }
        };
        
        e.EvaluateFunction += (s, args) =>
        {

            foreach (var noiseElement in noiseList)
            {
                if (s == noiseElement.Key)
                {
                    if (noiseElement.Value.dimension == 2)
                    {
                        args.Result = noiseElement.Value.noise.GetNoise((float) args.Parameters[0].Evaluate(), (float) args.Parameters[1].Evaluate()); 
                    }
                    else if (noiseElement.Value.dimension == 3)
                    {
                        args.Result = noiseElement.Value.noise.GetNoise((float) args.Parameters[0].Evaluate(), (float) args.Parameters[1].Evaluate(), (float) args.Parameters[2].Evaluate());
                    }
                }
            }
        };
        
        return Convert.ToDouble(e.Evaluate());
    }
}

public struct EquationNoise
{
    public int dimension;
    public FastNoiseLite noise;
}
