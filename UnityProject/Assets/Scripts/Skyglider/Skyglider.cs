using UnityEngine;

public class Skyglider : MonoBehaviour
{
    [Header("Mounting")]
    public Transform seatPoint;

    [Header("Flight")]
    public float cruiseSpeed = 18f;
    public float turnSpeed = 65f;
    public float pitchSpeed = 45f;
    public float climbStrength = 6f;
    public float glideFallSpeed = 2.5f;
    public float maxVerticalSpeed = 8f;

    private Player nearbyPlayer;
    private Player mountedPlayer;
    private PlayerSkygliderState mountedPlayerState;
    private Rigidbody gliderRigidbody;
    private float currentPitch;

    private void Awake()
    {
        Debug.Log("Skyglider -> Awake on " + name);
        gliderRigidbody = GetComponent<Rigidbody>();

        if (gliderRigidbody == null)
        {
            gliderRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        gliderRigidbody.useGravity = false;
        gliderRigidbody.linearDamping = 1.5f;
        gliderRigidbody.angularDamping = 2f;
        gliderRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
        gliderRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gliderRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        if (seatPoint == null)
        {
            seatPoint = transform;
            Debug.Log("Skyglider -> SeatPoint not assigned, defaulting to transform.");
        }
    }

    private void Update()
    {
        if (mountedPlayer == null)
        {
            if (nearbyPlayer != null && nearbyPlayer.InteractAction != null && nearbyPlayer.InteractAction.triggered)
            {
                Debug.Log("Skyglider -> interact pressed while player is nearby. Trying to mount.");
            }
            TryMountNearbyPlayer();
            return;
        }

        if (mountedPlayer.InteractAction != null && mountedPlayer.InteractAction.triggered)
        {
            Debug.Log("Skyglider -> interact pressed while mounted. Dismounting.");
            DismountPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (mountedPlayer == null)
            return;

        HandleFlight();
    }

    private void TryMountNearbyPlayer()
    {
        if (nearbyPlayer == null)
        {
            Debug.Log("Skyglider -> TryMountNearbyPlayer aborted. nearbyPlayer is null.");
            return;
        }

        if (nearbyPlayer.InteractAction == null || !nearbyPlayer.InteractAction.triggered)
        {
            return;
        }

        PlayerSkygliderState skygliderState = GetOrAddSkygliderState(nearbyPlayer);
        if (!skygliderState.HasSkyglider())
        {
            Debug.Log("Skyglider -> player does not have skyglider ownership yet.");
            return;
        }

        Debug.Log("Skyglider -> mounting player " + nearbyPlayer.name);
        MountPlayer(nearbyPlayer, skygliderState);
    }

    private void MountPlayer(Player player, PlayerSkygliderState skygliderState)
    {
        Debug.Log("Skyglider -> MountPlayer starting.");
        mountedPlayer = player;
        mountedPlayerState = skygliderState;

        skygliderState.MountSkyglider(this);
        player.transform.SetParent(seatPoint);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        Rigidbody playerRigidbody = GetPlayerRigidbody(player);
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = false;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        Collider playerCollider = GetPlayerCollider(player);
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        Debug.Log("Player mounted skyglider.");
    }

    private void DismountPlayer()
    {
        if (mountedPlayer == null)
        {
            Debug.Log("Skyglider -> DismountPlayer called but mountedPlayer is null.");
            return;
        }

        Player player = mountedPlayer;
        PlayerSkygliderState skygliderState = mountedPlayerState;

        player.transform.SetParent(null);
        player.transform.position = transform.position + transform.right * 2f + Vector3.down * 0.5f;

        Rigidbody playerRigidbody = GetPlayerRigidbody(player);
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            playerRigidbody.useGravity = true;
            playerRigidbody.linearVelocity = gliderRigidbody.linearVelocity;
        }

        Collider playerCollider = GetPlayerCollider(player);
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        if (skygliderState != null)
        {
            skygliderState.DismountSkyglider();
        }

        mountedPlayer = null;
        mountedPlayerState = null;
        currentPitch = 0f;

        Debug.Log("Player dismounted skyglider.");
    }

    private void HandleFlight()
    {
        Vector2 moveInput = mountedPlayer.moveAction != null ? mountedPlayer.moveAction.ReadValue<Vector2>() : Vector2.zero;

        Vector2 lookInput = mountedPlayer.orientationAction != null
            ? mountedPlayer.orientationAction.ReadValue<Vector2>()
            : Vector2.zero;

        float yawInput = moveInput.x;
        float pitchInput = -lookInput.y;

        transform.Rotate(0f, yawInput * turnSpeed * Time.fixedDeltaTime, 0f, Space.Self);

        currentPitch += pitchInput * pitchSpeed * Time.fixedDeltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -35f, 35f);

        Vector3 currentEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentPitch, currentEuler.y, 0f);

        float climbLift = Mathf.Sin(currentPitch * Mathf.Deg2Rad) * climbStrength;
        float verticalSpeed = Mathf.Clamp(-glideFallSpeed + climbLift, -maxVerticalSpeed, maxVerticalSpeed);

        Vector3 forwardVelocity = transform.forward * cruiseSpeed;
        Vector3 targetVelocity = new Vector3(forwardVelocity.x, verticalSpeed, forwardVelocity.z);

        gliderRigidbody.linearVelocity = targetVelocity;
    }

    private PlayerSkygliderState GetOrAddSkygliderState(Player player)
    {
        PlayerSkygliderState skygliderState = player.GetComponent<PlayerSkygliderState>();
        if (skygliderState == null)
        {
            skygliderState = player.gameObject.AddComponent<PlayerSkygliderState>();
        }

        return skygliderState;
    }

    private Rigidbody GetPlayerRigidbody(Player player)
    {
        if (player.thisObject != null)
        {
            return player.thisObject.GetComponent<Rigidbody>();
        }

        return player.GetComponent<Rigidbody>();
    }

    private Collider GetPlayerCollider(Player player)
    {
        if (player.thisObject != null)
        {
            return player.thisObject.GetComponent<Collider>();
        }

        return player.GetComponent<Collider>();
    }

    public void SetNearbyPlayer(Player player)
    {
        nearbyPlayer = player;
        Debug.Log("Skyglider -> SetNearbyPlayer called with " + player.name);

        PlayerSkygliderState skygliderState = GetOrAddSkygliderState(player);
        if (!skygliderState.HasSkyglider())
        {
            skygliderState.GrantSkyglider(this);
            Debug.Log("Skyglider ownership granted to player.");
        }
        else
        {
            Debug.Log("Skyglider -> player already owns this skyglider.");
        }
    }

    public void ClearNearbyPlayer(Player player)
    {
        if (nearbyPlayer == player)
        {
            nearbyPlayer = null;
            Debug.Log("Skyglider -> ClearNearbyPlayer cleared current nearby player.");
        }
    }
}
