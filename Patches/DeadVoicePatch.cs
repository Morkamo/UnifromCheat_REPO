using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(PlayerVoiceChat), "Update")]
internal static class DeadVoicePatch
{
    private static readonly HashSet<PlayerVoiceChat> AppliedVoiceChats = new();
    internal static readonly AccessTools.FieldRef<PlayerAvatar, bool> AvatarIsDisabled =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isDisabled");
    internal static readonly AccessTools.FieldRef<PlayerAvatar, bool> AvatarDeadSet =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("deadSet");
    internal static readonly AccessTools.FieldRef<PlayerAvatar, bool> AvatarIsLocal =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isLocal");

    private static void Postfix(PlayerVoiceChat __instance)
    {
        if (__instance == null)
            return;

        bool shouldApply = Core.isDeadVoiceEnabled && IsLocalAlive() && IsRemoteDeadVoice(__instance);
        if (shouldApply)
        {
            ApplyDeadVoice(__instance);
            AppliedVoiceChats.Add(__instance);
            return;
        }

        if (AppliedVoiceChats.Remove(__instance))
            RestoreGameVoiceRoute(__instance);
    }

    private static bool IsLocalAlive()
    {
        PlayerAvatar local = PlayerAvatar.instance;
        if (local == null)
            return false;

        return !AvatarIsDisabled(local) && !AvatarDeadSet(local);
    }

    private static bool IsRemoteDeadVoice(PlayerVoiceChat voiceChat)
    {
        PlayerAvatar avatar = PlayerVoiceChatRefs.playerAvatar(voiceChat);
        if (avatar == null || AvatarIsLocal(avatar))
            return false;

        return AvatarIsDisabled(avatar) || AvatarDeadSet(avatar);
    }

    private static void ApplyDeadVoice(PlayerVoiceChat voiceChat)
    {
        AudioSource source = PlayerVoiceChatRefs.audioSource(voiceChat);
        if (source != null)
        {
            source.outputAudioMixerGroup = PlayerVoiceChatRefs.distortedOverrideActive(voiceChat)
                ? voiceChat.mixerMicrophoneSoundDistorted
                : voiceChat.mixerMicrophoneSound;
            source.spatialBlend = 0f;
        }

        AudioLowPassLogic lowPass = PlayerVoiceChatRefs.lowPassLogic(voiceChat);
        if (lowPass != null)
            AudioLowPassLogicRefs.Volume(lowPass) = 1f;

        AudioSource ttsSource = PlayerVoiceChatRefs.ttsAudioSource(voiceChat);
        if (ttsSource != null)
        {
            ttsSource.outputAudioMixerGroup = voiceChat.mixerTTSSound;
            ttsSource.spatialBlend = 0f;
        }

        AudioLowPassLogic ttsLowPass = PlayerVoiceChatRefs.lowPassLogicTTS(voiceChat);
        if (ttsLowPass != null)
            AudioLowPassLogicRefs.Volume(ttsLowPass) = 1f;
    }

    private static void RestoreGameVoiceRoute(PlayerVoiceChat voiceChat)
    {
        AudioSource source = PlayerVoiceChatRefs.audioSource(voiceChat);
        if (source == null)
            return;

        if (PlayerVoiceChatRefs.inLobbyMixer(voiceChat))
        {
            source.outputAudioMixerGroup = voiceChat.mixerMicrophoneSpectate;

            AudioSource ttsSource = PlayerVoiceChatRefs.ttsAudioSource(voiceChat);
            if (ttsSource != null)
                ttsSource.outputAudioMixerGroup = voiceChat.mixerTTSSpectate;

            return;
        }

        source.outputAudioMixerGroup = PlayerVoiceChatRefs.distortedOverrideActive(voiceChat)
            ? voiceChat.mixerMicrophoneSoundDistorted
            : voiceChat.mixerMicrophoneSound;

        AudioSource normalTtsSource = PlayerVoiceChatRefs.ttsAudioSource(voiceChat);
        if (normalTtsSource != null)
            normalTtsSource.outputAudioMixerGroup = voiceChat.mixerTTSSound;
    }
}

[HarmonyPatch(typeof(MenuSpectateList), "Update")]
internal static class DeadVoiceSpectateListPatch
{
    private static bool wasForcedVisible;
    private static readonly AccessTools.FieldRef<MenuSpectateList, List<PlayerAvatar>> SpectatingPlayers =
        AccessTools.FieldRefAccess<MenuSpectateList, List<PlayerAvatar>>("spectatingPlayers");
    private static readonly AccessTools.FieldRef<MenuSpectateList, List<GameObject>> ListObjects =
        AccessTools.FieldRefAccess<MenuSpectateList, List<GameObject>>("listObjects");
    private static readonly MethodInfo PlayerAddMethod = AccessTools.Method(typeof(MenuSpectateList), "PlayerAdd");
    private static readonly MethodInfo PlayerRemoveMethod = AccessTools.Method(typeof(MenuSpectateList), "PlayerRemove");

    private static void Postfix(MenuSpectateList __instance)
    {
        if (__instance == null)
            return;

        if (Core.isDeadVoiceEnabled && IsLocalAlive())
        {
            SyncDeadPlayers(__instance);
            UpdateListOffset(__instance);
            __instance.Show();
            wasForcedVisible = true;
            return;
        }

        if (wasForcedVisible)
        {
            __instance.Hide();
            wasForcedVisible = false;
        }
    }

    private static bool IsLocalAlive()
    {
        PlayerAvatar local = PlayerAvatar.instance;
        if (local == null)
            return false;

        return !DeadVoicePatch.AvatarIsDisabled(local) && !DeadVoicePatch.AvatarDeadSet(local);
    }

    private static void SyncDeadPlayers(MenuSpectateList list)
    {
        List<PlayerAvatar> spectatingPlayers = SpectatingPlayers(list);
        if (spectatingPlayers == null)
            return;

        List<PlayerAvatar> players = SemiFunc.PlayerGetList();
        foreach (PlayerAvatar player in players)
        {
            if (player == null || DeadVoicePatch.AvatarIsLocal(player) || !IsDeadForDeadVoice(player))
                continue;

            if (!spectatingPlayers.Contains(player))
                PlayerAddMethod.Invoke(list, new object[] { player });
        }

        foreach (PlayerAvatar player in spectatingPlayers.ToList())
        {
            if (player == null || !players.Contains(player) || !IsDeadForDeadVoice(player))
                PlayerRemoveMethod.Invoke(list, new object[] { player });
        }
    }

    private static bool IsDeadForDeadVoice(PlayerAvatar player)
    {
        return DeadVoicePatch.AvatarIsDisabled(player) || DeadVoicePatch.AvatarDeadSet(player);
    }

    private static void UpdateListOffset(MenuSpectateList list)
    {
        List<GameObject> listObjects = ListObjects(list);
        int count = listObjects?.Count ?? 0;
        list.SemiUIScoot(new Vector2(0f, count * 22f), 0.2f);
    }
}
