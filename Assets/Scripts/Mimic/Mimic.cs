using UnityEngine;

public class Mimic : MonoBehaviour
{
	[SerializeField] private MimicEntity _mimic;

	public string Name { get; private set; }

	public float SpeedReduce { get; private set; }

	public float JumpReduce { get; private set; }

	public GameObject Sprite { get; private set; }

	void Start()
	{
		Name = _mimic.Name;
		SpeedReduce = _mimic.PlayerSpeedMultiplier;
		JumpReduce = _mimic.JumpSpeedMultiplier;
		Sprite = _mimic.Sprite;
	}
}
