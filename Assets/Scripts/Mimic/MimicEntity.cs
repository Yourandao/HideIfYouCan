using UnityEngine;

[CreateAssetMenu(fileName = "New Mimic", menuName = "Mimic Object")]
public class MimicEntity : ScriptableObject
{
	public string Name;

	public float PlayerSpeedMultiplier;

	public float JumpSpeedMultiplier;

	public Mesh Sprite;
}
