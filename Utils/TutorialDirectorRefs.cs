using System.Collections.Generic;
using HarmonyLib;

namespace UnifromCheat_REPO.Utils;

internal static class TutorialDirectorRefs
{
    // internal
    public static readonly AccessTools.FieldRef<TutorialDirector, int> currentPage =
        AccessTools.FieldRefAccess<TutorialDirector, int>("currentPage");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> tutorialActive =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("tutorialActive");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> tutorialProgress =
        AccessTools.FieldRefAccess<TutorialDirector, float>("tutorialProgress");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> deadPlayer =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("deadPlayer");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerSprinted =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerSprinted");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerJumped =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerJumped");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerSawHead =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerSawHead");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerRevived =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerRevived");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerHealed =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerHealed");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerRotated =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerRotated");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerTumbled =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerTumbled");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerCrouched =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerCrouched");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerCrawled =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerCrawled");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerUsedCart =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerUsedCart");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerPushedAndPulled =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerPushedAndPulled");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerUsedToggle =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerUsedToggle");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerHadItemsAndUsedInventory =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerHadItemsAndUsedInventory");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerUsedMap =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerUsedMap");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerUsedChargingStation =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerUsedChargingStation");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerReviveTipDone =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerReviveTipDone");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerChatted =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerChatted");
    public static readonly AccessTools.FieldRef<TutorialDirector, bool> playerUsedExpression =
        AccessTools.FieldRefAccess<TutorialDirector, bool>("playerUsedExpression");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutChatting =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutChatting");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutCharging =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutCharging");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutMap =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutMap");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutInventory =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutInventory");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutCart =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutCart");
    public static readonly AccessTools.FieldRef<TutorialDirector, int> numberOfRoundsWithoutToggle =
        AccessTools.FieldRefAccess<TutorialDirector, int>("numberOfRoundsWithoutToggle");
    public static readonly AccessTools.FieldRef<TutorialDirector, List<string>> potentialTips =
        AccessTools.FieldRefAccess<TutorialDirector, List<string>>("potentialTips");
    public static readonly AccessTools.FieldRef<TutorialDirector, List<string>> shownTips =
        AccessTools.FieldRefAccess<TutorialDirector, List<string>>("shownTips");

    // private
    public static readonly AccessTools.FieldRef<TutorialDirector, float> tutorialCheckActiveTimer =
        AccessTools.FieldRefAccess<TutorialDirector, float>("tutorialCheckActiveTimer");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> tutorialActiveTimer =
        AccessTools.FieldRefAccess<TutorialDirector, float>("tutorialActiveTimer");
    public static readonly AccessTools.FieldRef<TutorialDirector, PhysGrabCart> tutorialCart =
        AccessTools.FieldRefAccess<TutorialDirector, PhysGrabCart>("tutorialCart");
    public static readonly AccessTools.FieldRef<TutorialDirector, ExtractionPoint> tutorialExtractionPoint =
        AccessTools.FieldRefAccess<TutorialDirector, ExtractionPoint>("tutorialExtractionPoint");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> arrowDelay =
        AccessTools.FieldRefAccess<TutorialDirector, float>("arrowDelay");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> showTipTimer =
        AccessTools.FieldRefAccess<TutorialDirector, float>("showTipTimer");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> showTipTime =
        AccessTools.FieldRefAccess<TutorialDirector, float>("showTipTime");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> delayBeforeTip =
        AccessTools.FieldRefAccess<TutorialDirector, float>("delayBeforeTip");
    public static readonly AccessTools.FieldRef<TutorialDirector, string> scheduleTipName =
        AccessTools.FieldRefAccess<TutorialDirector, string>("scheduleTipName");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> scheduleTipTimer =
        AccessTools.FieldRefAccess<TutorialDirector, float>("scheduleTipTimer");
    public static readonly AccessTools.FieldRef<TutorialDirector, float> scheduleTipShowTimer =
        AccessTools.FieldRefAccess<TutorialDirector, float>("scheduleTipShowTimer");
}