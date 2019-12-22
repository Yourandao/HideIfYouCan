using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Interact
{
	public class PlayerInteracting : MonoBehaviour
	{
		[SerializeField] private float _viewRange = 100f;

		[SerializeField] private MeshFilter _renderer;

		private void Update()
		{
			//Debug.DrawRay(transform.position, transform.forward, Color.red, _viewRange);
			if (Input.GetButtonDown("Attack"))
			{
				Transform();
			}
		}

		private void Transform()
		{
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _viewRange))
			{
				var hitObject = hit.transform.GetComponent<Mimic>();

				Debug.Log(hitObject?.Name ?? "null");

				if (hitObject != null)
				{
					_renderer.mesh = hitObject.Sprite;
				}
			}
		}
	}
}
