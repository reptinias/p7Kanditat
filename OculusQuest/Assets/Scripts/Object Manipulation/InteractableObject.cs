using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    Outline outline;
    List<RigSelection> boneTargets = new List<RigSelection>();

    public int test = 3;

    private void Update()
    {
        
    }

    public void SetOutLine(Outline outline)
    {
        this.outline = outline;
    }

    public void EnableOutline(bool enabled)
    {
        outline.enabled = enabled;

        foreach (RigSelection bone in boneTargets)
        {
            bone.EnableSphere(enabled);
        }
    }

    public void AddBone(RigSelection rs)
    {
        boneTargets.Add(rs);
    }

    public List<RigSelection> GetBoneTargets()
    {
        return boneTargets;
    }
}