using UnityEngine;
using System.Collections.Generic;

class Point {
	public Vector3 p;
	public Point next;
}

public class LinesGR : MonoBehaviour {


	private Mesh ml;
	public Material lmat;
	
	private Mesh ms;
	public Material smat;
	
	private Vector3 s;

	public float lineSize = 0.03f;

	
	private Point first;
	
	private float speed = 5.0f;


	void Start () {
		ml = new Mesh();
		ms = new Mesh();
	}

	
	void Draw() {
		Graphics.DrawMesh(ml, transform.localToWorldMatrix, lmat, 0);
	}
	
	Vector3[] MakeQuad(Vector3 s, Vector3 e, float w) {
		w = w / 2;
		Vector3[] q = new Vector3[4];

		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();
		
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s + l * -w);
		q[2] = transform.InverseTransformPoint(e + l * w);
		q[3] = transform.InverseTransformPoint(e + l * -w);

		return q;
	}
	
	void AddLine(Mesh m, Vector3[] quad, bool tmp) {
			int vl = m.vertices.Length;
			
			Vector3[] vs = m.vertices;
			if(!tmp || vl == 0) vs = resizeVertices(vs, 4);
			else vl -= 4;
			
			vs[vl] = quad[0];
			vs[vl+1] = quad[1];
			vs[vl+2] = quad[2];
			vs[vl+3] = quad[3];
			
			int tl = m.triangles.Length;
			
			int[] ts = m.triangles;
			if(!tmp || tl == 0) ts = resizeTraingles(ts, 6);
			else tl -= 6;
			ts[tl] = vl;
			ts[tl+1] = vl+1;
			ts[tl+2] = vl+2;
			ts[tl+3] = vl+1;
			ts[tl+4] = vl+3;
			ts[tl+5] = vl+2;
			
			m.vertices = vs;
			m.triangles = ts;
			m.RecalculateBounds();
	}
	
	void processInput() {

		
		if(Input.GetKeyDown(KeyCode.C)) {
			ml = new Mesh();
			ms = new Mesh();
			transform.rotation = Quaternion.identity;
			first = null;
		}
	}
	
	Vector3 GetNewPoint() {
		return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1.0f));
	}
	
	Vector3[] resizeVertices(Vector3[] ovs, int ns) {
		Vector3[] nvs = new Vector3[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
	int[] resizeTraingles(int[] ovs, int ns) {
		int[] nvs = new int[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}

	/** Replace the Update function with this one for a click&drag drawing option */
	void Update() {
		processInput();
		
		Vector3 e;
		
		if(Input.GetMouseButtonDown(0)) {
			s = transform.InverseTransformPoint(GetNewPoint());
		}
		
		if(Input.GetMouseButton(0)) {
			e = GetNewPoint();
			AddLine(ml, MakeQuad(transform.TransformPoint(s), e, lineSize), true);
		}

		if(Input.GetMouseButtonUp(0)) {
			e = GetNewPoint();
			AddLine(ml, MakeQuad(transform.TransformPoint(s), e, lineSize), false);
		}
		
		Draw();
	}

}







