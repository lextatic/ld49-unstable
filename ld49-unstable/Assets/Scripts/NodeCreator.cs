using UnityEngine;

public class NodeCreator : MonoBehaviour
{
	public GameObject NodePrefab;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

			Instantiate(NodePrefab, point, Quaternion.identity);
		}
	}
}
