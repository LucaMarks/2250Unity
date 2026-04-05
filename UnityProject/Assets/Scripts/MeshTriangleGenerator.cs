using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // private float width = 1;
    // private float height = 1;
    // private float depth = 1;

    public Material material;
    public GameObject parentObject;

    private Vector3[] verticies = new Vector3[4];
    private Vector2[] uv = new Vector2[4];
    private int[] triangles = new int[3];

    private GameObject meshObject;
    private GameObject meshObject2;
    private Mesh mesh;

    void Start()
    {
        generateMeshData();

        mesh = new Mesh();
        mesh.name = "Triangle Mesh";

        meshObject = new GameObject("Triangle", typeof(MeshRenderer), typeof(MeshFilter));
        meshObject2 = new GameObject("Triangle Mesh2", typeof(MeshRenderer), typeof(MeshFilter));

        meshObject.GetComponent<MeshFilter>().mesh = mesh;
        meshObject.GetComponent<MeshRenderer>().material = material;
        meshObject2.GetComponent<MeshFilter>().mesh = mesh;
        meshObject2.GetComponent<MeshRenderer>().material = material;

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;

        Debug.Log("Generated meshes");
        meshObject.transform.SetParent(parentObject.transform);
        meshObject2.transform.SetParent(parentObject.transform);

        Vector3 moveVec = new Vector3(21, 0, 3);
        meshObject.transform.position = parentObject.transform.position + moveVec;
        meshObject2.transform.position = parentObject.transform.position + moveVec; 
        meshObject2.transform.rotation = Quaternion.Euler(0, 0, 90);
        
        // Mesh mesh = new Mesh();
        // Vector3[] verticies = new Vector3[4];
        //
        // verticies[0] = new Vector3(0, 0, 0);
        // verticies[1] = new Vector3(width, height, depth);
        // verticies[2] = new Vector3(-width, -height, depth);
        // verticies[3] = new Vector3(width, -height, -depth);
        //
        // mesh.vertices = verticies; 
        // mesh.triangles = new int[] {0, 1, 2, 0, 1, 3, 0, 2, 3, 1, 2, 3};
        // mesh.RecalculateNormals();
        
    }

    private void generateMeshData()
    {
        verticies[0] = new Vector3(0, 0, 0);
        verticies[1] = new Vector3(0, 1, 0);
        verticies[2] = new Vector3(1, 0, 0);
        // verticies[3] = new Vector3(1, 0, 0);

        triangles[0] = 0;
        triangles[1] = 2; 
        triangles[2] = 1;

        // triangles[3] = 0;
        // triangles[4] = 2;
        // triangles[5] = 3;

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
