using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hang.AiryAudio;

public class CS_PlayerAnimationEvents : MonoBehaviour {


    public void PlayFootStepSound() {
        AiryAudioManager.Instance.GetAudioData("FootStepSounds").Play(this.transform.position);

    }
}
