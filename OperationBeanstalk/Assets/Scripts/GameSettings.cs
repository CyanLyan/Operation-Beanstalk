using System;
using UnityEngine;

public class GameSettings: MonoBehaviour
{
	public GameObject _blockSettingsObject;
    private BlockSettings _blockSettings;

	public float _PlayerNudgeForce;

    private void Awake()
    {
        BlockSettings = _blockSettingsObject.GetComponent<BlockSettings>();
    }
    public GameSettings(BlockSettings blockSettings, float playerNudgeForce)
    {
        _blockSettings = blockSettings;
        _PlayerNudgeForce = playerNudgeForce;
    }

    public BlockSettings BlockSettings {  get { return _blockSettings; }  set { _blockSettings = value; } }
}
