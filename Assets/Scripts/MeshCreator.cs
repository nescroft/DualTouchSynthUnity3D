using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCreator : MonoBehaviour {

    public float roundness, indentation;
    public int xSize, ySize, zSize;
    public float position;
	public float rotationX = 7.0f;
	public float rotationY = 7.0f;
	public float rotationZ = 7.0f;
	public float xFloat, yFloat, zFloat;
    private Mesh myMesh;
    private Vector3[] verts;
    private Vector3[] normals;
    private int[] tris;
    private int t_increase = 0;                                             //t_increase: Used to keep track of triangle array indicies

    // Generate Mesh right away
    void Awake(){
        Generate();
    }

    // Use this for initialization
    void Start () {
		
	}

    private void Generate() {
        GetComponent<MeshFilter>().mesh = myMesh = new Mesh();
        CreateVerts();
        CreateTris();
    }

    //CreateVerts calculates the exact position where each verticy will start
    private void CreateVerts() {
        int corners = 8;
        int edges = 4 * (xSize + ySize + zSize - 3);
        int faceXY = (xSize - 1) * (ySize - 1);
        int faceXZ = (xSize - 1) * (zSize - 1);
        int faceYZ = (ySize - 1) * (zSize - 1);
        int faces = 2 * (faceXY + faceXZ + faceYZ);

        verts = new Vector3[faces + edges + corners];
        normals = new Vector3[verts.Length];

        if (roundness <= 0) {
            roundness = 1;
        }

        int i = 0; 
        for (int y = 0; y < (ySize+1); y++) {
            for (int x = 0; x < (xSize + 1); x++) {
                SetVerts(i++,x,y,0);
            }
            for (int z = 1; z < (zSize + 1); z++)
            {
                SetVerts(i++, xSize, y, z);
            }
            for (int x = (xSize - 1); x >= 0; x--)
            {
                SetVerts(i++, x, y, zSize);
            }
            for (int z = (zSize - 1); z > 0; z--)
            {
                SetVerts(i++, 0, y, z);
            }
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVerts(i++, x, ySize, z);
            }
        }
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVerts(i++, x, 0, z);
            }
        }

        ShiftVerts();

        myMesh.vertices = verts;
        myMesh.normals = normals;

    }

    //SetVerts will create a Vector3 and assign it to the corresponding position in the Mesh's verticy array
    //This will also alter the vertices based off of shape altering variables
    private void SetVerts(int i, int x, int y, int z){
        Vector3 fvect = verts[i] = new Vector3(x, y, z);
        
        if (x < roundness)
        {
            fvect.x = roundness;
        }
        else if (x > xSize - roundness)
        {
            fvect.x = xSize - roundness;
        }
        if (y < roundness)
        {
            fvect.y = roundness;
        }
        else if (y > ySize - roundness)
        {
            fvect.y = ySize - roundness;
        }
        if (z < roundness)
        {
            fvect.z = roundness;
        }
        else if (z > ySize - roundness)
        {
            fvect.z = zSize - roundness;
        }
        normals[i] = (verts[i] - fvect).normalized;
        verts[i] = fvect + normals[i] * roundness;

        //ShiftMiddleVerts(i,x,y,z);
    }

    private void ShiftMiddleVerts(int i, int x, int y, int z){
        if (y == ySize/2) {
            if (x == 0) {
                verts[i].x++;
            }
            else if (x == xSize)
            {
                verts[i].x--;
            }
            if (z == 0)
            {
                verts[i].z++;
            }
            else if (z == zSize - 1)
            {
                verts[i].z--;
            }
        }
    }

    private void ShiftVerts() {
        for (int i = 0; i < verts.Length; i++) {
            Vector3 shiftvert = verts[i];
            shiftvert.x -= ((float)xSize)/2;
            shiftvert.y -= ((float)ySize)/2;
            shiftvert.z -= ((float)zSize)/2;
            verts[i] = shiftvert;
        }
    }


    private void CreateTris() {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] triangles = new int[quads * 6];
        int ring = (xSize + zSize) * 2;
        int t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++) {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                t += 6;
            }
            SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            t += 6;
        }
        CreateTopFace(triangles, t, ring);
        t += t_increase;
        CreateBottomFace(triangles, t, ring);
        myMesh.triangles = triangles;
    }

    private void SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
    }

    private void CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            t += 6;
            t_increase += 6;
        }
        SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
        t += 6;
        t_increase += 6;

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;
        for (int z = 1; z < zSize - 1; z++, vMax++, vMin--, vMid++)
        {
            SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            t += 6;
            t_increase += 6;
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
                t += 6;
                t_increase += 6;
            }
            SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
            t += 6;
            t_increase += 6;
        }
        SetQuad(triangles, t, vMin, vMid, vMin - 1, vMin - 2);
        t += 6;
        t_increase += 6;

        vMin -= 2;
        for (int x = 1; x < xSize - 1; x++, vMid++, vMin--)
        {
            SetQuad(triangles, t, vMid, vMid + 1, vMin, vMin - 1);
            t += 6;
            t_increase += 6;
        }
        SetQuad(triangles, t, vMid, vMax, vMin, vMin - 1);
        t += 6;
        t_increase += 6;
    }

    private void CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = verts.Length - (xSize - 1) * (zSize - 1);
        SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        t += 6;
        t_increase += 6;
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            t += 6;
            t_increase += 6;
        }
        SetQuad(triangles, t, vMid, v + 2, v, v + 1);
        t += 6;
        t_increase += 6;

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            t += 6;
            t_increase += 6;
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                SetQuad(triangles, t,vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
                t += 6;
                t_increase += 6;
            }
            SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
            t += 6;
            t_increase += 6;
        }

        int vTop = vMin - 1;
        SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        t += 6;
        t_increase += 6;
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            t += 6;
            t_increase += 6;
        }
        SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
        t += 6;
        t_increase += 6;
    }

    // Update is called once per frame
    void Update()
    {
        CreateVerts();
        Rotate(rotationX,rotationY,rotationZ);
        //ChangeSize();
		RoundnessSlider(roundness);
    }

    public void RoundnessSlider(float slider_val) {
        roundness = slider_val;
    }

    public void Rotate(float x, float y, float z) {
        transform.Rotate(new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime));
    }

	public void ChangeSize() {
        transform.localScale = new Vector3(xFloat, yFloat, zFloat);
    }

    /*
    private void OnDrawGizmos()
    {
        if (verts == null || normals == null) {
            return;
        }
        for (int i = 0; i < verts.Length; i++ ) {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(verts[i],0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(verts[i], normals[i]);
        }
    }
    */
}
