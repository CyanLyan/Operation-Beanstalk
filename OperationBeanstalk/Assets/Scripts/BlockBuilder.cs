using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BlockBuilder
{
    public static GameObject randomizeBlockDimensions(GameObject block, float RandomnessIndex)
    {
        var randomWeightModifierMaxVariation = RandomnessIndex / 10;
        var randomScaleModifierMaxVariation = RandomnessIndex / 1000;
        float randomWeightModifier = Random.Range(0, randomWeightModifierMaxVariation);
        float randomScaleModifier = Random.Range(0, randomScaleModifierMaxVariation);


        block.GetComponent<Rigidbody>().mass = GenerateRandomDeviation(block.GetComponent<Rigidbody>().mass, randomWeightModifier);
        //Vector3 randomScaleDimensions = new Vector3(GenerateRandomDeviation(block.transform.localScale.x, randomScaleModifier),
        //    GenerateRandomDeviation(block.transform.localScale.y, randomScaleModifier), GenerateRandomDeviation(block.transform.localScale.z, randomScaleModifier));
        //block.transform.localScale = randomScaleDimensions;

        return block;
    }

    private static float GenerateRandomDeviation(float dimensionToDeviate, float randomWeightModifierMaxVariation)
    {
        float randomModifier = Random.Range(0, randomWeightModifierMaxVariation);

        //Do coin flip
        if (Mathf.Round(Random.Range(0, 2)) == 1)
        {
            dimensionToDeviate = dimensionToDeviate - randomModifier;
        }
        //else
        //{
        //    dimensionToDeviate = dimensionToDeviate - randomModifier; 
        //}
        return dimensionToDeviate;
    }

    private static GameObject SetRigidBodyDimensions(GameObject block, BlockSettings blockSettings)
    {
        var rigidBody = block.GetComponent<Rigidbody>();
        rigidBody.mass = blockSettings.Mass;
        rigidBody.drag = blockSettings.Drag;
        rigidBody.angularDrag = blockSettings.AngularDrag;
        rigidBody.useGravity = true;
        return block;
    }

    private static GameObject SetBlockClassProperties(GameObject block, BlockSettings blockSettings)
    {
        var blockClass = block.GetComponent<Block>();
        blockClass.blockIsInTowerZone = true;
        blockClass.userCanDrag = true;
        blockClass.userCanNudge = true;
        return block;
    }

    public static GameObject SetNonUniqueBlockSettings(GameObject block, BlockSettings blockSettings)
    {
        block = SetBlockClassProperties(block, blockSettings);
        block = SetRigidBodyDimensions(block, blockSettings);
        return block;
    }
}
