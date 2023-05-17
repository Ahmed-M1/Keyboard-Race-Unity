using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;


/*
 * 
 * 
 */
public class playerSettings : NetworkBehaviour
{

    [SerializeField] private TextMeshProUGUI ID;
    [SerializeField] private NetworkVariable<FixedString128Bytes> networkPlayername = new NetworkVariable<FixedString128Bytes>("P0:", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public override void OnNetworkSpawn()
    {
        networkPlayername.Value = "P" + (OwnerClientId + 1) + ":";
        ID.text = networkPlayername.Value.ToString();
    }




}
