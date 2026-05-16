using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace UnifromCheat_REPO.Utils;

internal static class PlayerVoiceChatRefs
{
    public static readonly AccessTools.FieldRef<PlayerVoiceChat, Photon.Pun.PhotonView> photonView =
        AccessTools.FieldRefAccess<PlayerVoiceChat, Photon.Pun.PhotonView>("photonView");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, PlayerAvatar> playerAvatar =
        AccessTools.FieldRefAccess<PlayerVoiceChat, PlayerAvatar>("playerAvatar");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, AudioSource> audioSource =
        AccessTools.FieldRefAccess<PlayerVoiceChat, AudioSource>("audioSource");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, AudioSource> ttsAudioSource =
        AccessTools.FieldRefAccess<PlayerVoiceChat, AudioSource>("ttsAudioSource");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, AudioLowPassLogic> lowPassLogic =
        AccessTools.FieldRefAccess<PlayerVoiceChat, AudioLowPassLogic>("lowPassLogic");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, AudioLowPassLogic> lowPassLogicTTS =
        AccessTools.FieldRefAccess<PlayerVoiceChat, AudioLowPassLogic>("lowPassLogicTTS");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, bool> distortedOverrideActive =
        AccessTools.FieldRefAccess<PlayerVoiceChat, bool>("distortedOverrideActive");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, bool> inLobbyMixer =
        AccessTools.FieldRefAccess<PlayerVoiceChat, bool>("inLobbyMixer");

    public static readonly AccessTools.FieldRef<PlayerVoiceChat, bool> isTalking =
        AccessTools.FieldRefAccess<PlayerVoiceChat, bool>("isTalking");
}
