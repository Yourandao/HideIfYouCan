using Assets.Scripts.PlayerScripts.Control;
using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Interact
{
	public class PlayerInteracting : MonoBehaviour
	{
		[SerializeField] private float _viewRange = 100f;

		[SerializeField] private Transform _modelHolder;

		[SerializeField] private GameObject _model;

		[SerializeField] private GameObject _player;

		private float _initialSpeed;

		private float _initialJumpSpeed;

		private PlayerController _playerController;

		private void Start()
		{
			_playerController = _player.GetComponent<PlayerController>();

			_initialSpeed = _playerController.Speed;
			_initialJumpSpeed = _playerController.JumpSpeed;
		}

		private void Update()
		{
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

				if (hitObject != null)
				{
					Debug.Log(hitObject.Name);

					Destroy(_model);

					_model = Instantiate(hitObject.Sprite, _modelHolder);
					_model.AddComponent<Rigidbody>().isKinematic = true;

					if (_model.GetComponent<MeshCollider>() == null)
					{
						_model.AddComponent<MeshCollider>();
					}

					var characterController = _player.GetComponent<CharacterController>();

					characterController.center = _player.transform.localScale / 2;
					characterController.height = 0;

					_playerController.Speed = _initialSpeed * hitObject.SpeedReduce;
					_playerController.JumpSpeed = _initialJumpSpeed * hitObject.JumpReduce;
				}
			}
		}
	}
}
