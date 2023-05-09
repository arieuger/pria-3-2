using Unity.Netcode;
using UnityEngine;

namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                RandomMove();
            }
        }

        public void RandomMove() {
            if (NetworkManager.Singleton.IsServer) {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else {
                SubmitRandomPositionRequestServerRpc();
            }
        }

        public void Move(Vector3 direction) {
            
            if (!IsOwner) return;

            if (NetworkManager.Singleton.IsServer) {
                transform.position += direction;
                Position.Value = transform.position;
            } else {
                MoveServerRpc(direction);
            }
        }

        [ServerRpc]
        void SubmitRandomPositionRequestServerRpc(ServerRpcParams rpcParams = default) {
            Position.Value = GetRandomPositionOnPlane();
        }

        [ServerRpc]
        void MoveServerRpc(Vector3 direction, ServerRpcParams rpcParams = default) {
            Position.Value += direction;
        }

        static Vector3 GetRandomPositionOnPlane() {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update() {

            if (Input.GetKeyDown(KeyCode.W)) Move(Vector3.forward);
            if (Input.GetKeyDown(KeyCode.S)) Move(Vector3.back);
            if (Input.GetKeyDown(KeyCode.A)) Move(Vector3.left);
            if (Input.GetKeyDown(KeyCode.D)) Move(Vector3.right);

            transform.position = Position.Value;
        }
    }
}