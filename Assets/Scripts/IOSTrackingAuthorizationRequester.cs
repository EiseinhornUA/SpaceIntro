#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;

public class IOSTrackingAuthorizationRequester : MonoBehaviour
{
    private void Start()
    {
#if UNITY_IOS
        SkAdNetworkBinding.SkAdNetworkRegisterAppForNetworkAttribution();

        // check with iOS to see if the user has accepted or declined tracking
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            Debug.Log("Unity iOS Support: Requesting iOS App Tracking Transparency native dialog.");
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}