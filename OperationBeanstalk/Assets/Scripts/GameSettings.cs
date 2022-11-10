using System;
using UnityEngine;

public class GameSettings: MonoBehaviour
{
	public GameObject _blockSettingsObject;
    private BlockSettings _blockSettings;
    
    public int _nPalletsHigh;

    public float _PlayerNudgeForce;

    private void Awake()
    {
        BlockSettings = _blockSettingsObject.GetComponent<BlockSettings>();
    }
    public GameSettings(BlockSettings blockSettings, float playerNudgeForce, int nPalletsHigh)
    {
        _blockSettings = blockSettings;
        _PlayerNudgeForce = playerNudgeForce;
        _nPalletsHigh = nPalletsHigh;
    }

    public BlockSettings BlockSettings {  get { return _blockSettings; }  set { _blockSettings = value; } }

    public int NPalletsHigh { get { return _nPalletsHigh; } set { _nPalletsHigh = value; } }
}
