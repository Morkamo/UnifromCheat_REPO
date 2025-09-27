/*
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Utils
{
    public class PlayersTeleporter : MonoBehaviour
    {
        // ================= PUBLIC BUTTON ACTIONS =================

        /// <summary>
        /// Переместить себя к случайному другому игроку
        /// </summary>
        public static void GoToAny()
        {
            var players = GetOtherPlayers();
            if (players.Count == 0)
            {
                FireLog("[PT] No other players found.");
                return;
            }

            var rnd = new System.Random();
            var target = players[rnd.Next(players.Count)];

            ForceTeleport(PlayerController.instance.playerAvatar.gameObject, target.transform.position);
            FireLog($"[PT] Local player teleported to {target.name}");
        }

        /// <summary>
        /// Переместить случайного игрока к себе
        /// </summary>
        public static void AnyToYou()
        {
            var players = GetOtherPlayers();
            if (players.Count == 0)
            {
                FireLog("[PT] No other players found.");
                return;
            }

            var rnd = new System.Random();
            var target = players[rnd.Next(players.Count)];

            Vector3 myPos = PlayerController.instance.playerAvatar.transform.position;
            ForceTeleport(target.gameObject, myPos);

            FireLog($"[PT] {target.name} teleported to local player");
        }

        /// <summary>
        /// Переместить всех игроков к себе
        /// </summary>
        public static void AllToYou()
        {
            var players = GetOtherPlayers();
            if (players.Count == 0)
            {
                FireLog("[PT] No other players found.");
                return;
            }

            Vector3 myPos = PlayerController.instance.playerAvatar.transform.position;

            foreach (var pl in players)
            {
                ForceTeleport(pl.gameObject, myPos);
                FireLog($"[PT] {pl.name} teleported to local player");
            }
        }

        // ================= INTERNAL UTILS =================

        /// <summary>
        /// Получить список игроков кроме локального
        /// </summary>
        private static List<PlayerAvatar> GetOtherPlayers()
        {
            int localActor = PlayerController.instance.playerAvatar.GetPhotonView().OwnerActorNr;

            return GameDirector.instance.PlayerList
                .Where(pl => pl != null && pl.photonView != null && pl.photonView.OwnerActorNr != localActor)
                .ToList();
        }

        /// <summary>
        /// Жёсткий телепорт + RPC
        /// </summary>
        private static void ForceTeleport(GameObject go, Vector3 pos)
        {
            if (go == null) return;

            // локально двигаем
            go.transform.position = pos;

            // синхронизируем по сети
            var pv = go.GetComponent<PhotonView>();
            if (pv != null && PhotonNetwork.IsMasterClient)
                pv.RPC("ForceTeleportRPC", RpcTarget.All, pos);
        }
    }

    // ================ RPC HANDLER =================
    public class PlayerTeleportHandler : MonoBehaviourPun
    {
        [PunRPC]
        public void ForceTeleportRPC(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}
*/
