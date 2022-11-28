 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClickManager : MonoBehaviour
{
    [SerializeField] Material mat;
    Camera cam;
    int triangleIdx;
    Mesh mesh;
    int subMeshesNr;
    int matI = -1;
    Renderer rend;
    MeshCollider meshCollider;
    Ray ray;

    public SkinnedMeshRenderer meshRenderer;
    public MeshCollider collider;

    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }

    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        //UpdateCollider();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider != null || meshCollider.sharedMesh != null)
                {
                    mesh = meshCollider.sharedMesh;
                    Renderer renderer = hit.collider.GetComponent<SkinnedMeshRenderer>();
     
                    int[] hitTriangle = new int[]
                    {
                        mesh.triangles[hit.triangleIndex * 3],
                        mesh.triangles[hit.triangleIndex * 3 + 1],
                        mesh.triangles[hit.triangleIndex * 3 + 2]
                    };
                    for (int i = 0; i < mesh.subMeshCount; i++)
                    {
                        print(mesh.subMeshCount);
                        int[] subMeshTris = mesh.GetTriangles(i);
                        for (int j = 0; j < subMeshTris.Length; j += 3)
                        {
                            if (subMeshTris[j] == hitTriangle[0] &&
                                subMeshTris[j + 1] == hitTriangle[1] &&
                                subMeshTris[j + 2] == hitTriangle[2])
                            {
                                mat = renderer.materials[i];
                            }
                        }
                    }
                }
            }
        }
    }
}
 
 
