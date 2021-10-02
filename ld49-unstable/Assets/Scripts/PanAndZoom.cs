using Cinemachine;
using UnityEngine;

public class PanAndZoom : MonoBehaviour
{
	[SerializeField]
	private float _panSpeed = 10f;
	[SerializeField]
	private float _panMinXPosition = -20f;
	[SerializeField]
	private float _panMaxXPosition = 20f;
	[SerializeField]
	private float _panMinYPosition = -5f;
	[SerializeField]
	private float _panMaxYPosition = 20f;
	[SerializeField]
	private float _zoomSpeed = 10f;
	[SerializeField]
	private float _zoomInMax = 5f;
	[SerializeField]
	private float _zoomOutMax = 20f;

	private CinemachineInputProvider _inputProvider;
	private CinemachineVirtualCamera _virtualCamera;
	private Transform _cameraTransform;

	private void Awake()
	{
		_inputProvider = GetComponent<CinemachineInputProvider>();
		_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		_cameraTransform = _virtualCamera.VirtualCameraGameObject.transform;
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		float x = _inputProvider.GetAxisValue(0);
		float y = _inputProvider.GetAxisValue(1);
		float z = _inputProvider.GetAxisValue(2);

		if (x != 0 || y != 0)
		{
			PanScren(x, y);
		}

		if (z != 0)
		{
			ZoomScreen(z);
		}
	}

	public Vector2 PanDirection(float x, float y)
	{
		Vector2 direction = Vector2.zero;

		if (y >= Screen.height * 0.95f && y < Screen.height * 1.2f)
		{
			direction.y += 1;
		}
		else if (y <= Screen.height * 0.05f && y > 0 - Screen.height * 0.2f)
		{
			direction.y -= 1;
		}

		if (x >= Screen.width * 0.95f && x < Screen.width * 1.2f)
		{
			direction.x += 1;
		}
		else if (x <= Screen.width * 0.05f && x > 0 - Screen.width * 0.2f)
		{
			direction.x -= 1;
		}

		return direction;
	}

	public void ZoomScreen(float increment)
	{
		float fov = _virtualCamera.m_Lens.OrthographicSize;
		float target = Mathf.Clamp(fov + increment, _zoomInMax, _zoomOutMax);
		_virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(fov, target, _zoomSpeed * Time.deltaTime);
	}

	public void PanScren(float x, float y)
	{
		Vector2 direction = PanDirection(x, y);
		var targetPosition = new Vector3(Mathf.Clamp(_cameraTransform.position.x + direction.x, _panMinXPosition, _panMaxXPosition),
			Mathf.Clamp(_cameraTransform.position.y + direction.y, _panMinYPosition, _panMaxYPosition),
			_cameraTransform.position.z);
		_cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, _panSpeed * Time.deltaTime);
	}
}
