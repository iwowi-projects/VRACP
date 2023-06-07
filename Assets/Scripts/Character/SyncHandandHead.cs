using UnityEngine;



public class SyncHandAndHead : MonoBehaviour
{
    public VRMap head, leftHand, rightHand;
    public Transform headTarget;
    private Vector3 headBodyOffset;
    public float turnSmoothness = 2;

    // Start is called before the first frame update
    void Start()
    {
        //headBodyOffset = transform.position - headTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = headTarget.position + headBodyOffset;
        //transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headTarget.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }


    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget, rigTarget;
        public Vector3 trackingPositionOffset, trackingRotationOffset;

        public void Map()
        {
            rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }
}
