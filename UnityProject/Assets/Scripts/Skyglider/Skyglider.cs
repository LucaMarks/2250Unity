using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]


public class Skyglider : MonoBehaviour
{
    [Header("Mounting")]
    public Transform seatPoint;
    public Vector3 dismountOffset = new Vector3(2f, 1f, 0f);
    public Collider[] ignoredAutoDismountColliders;

    [Header("Flight")]
    public float cruiseSpeed = 10f;
    public float turnSpeed = 65f;
    public float pitchSpeed = 45f;
    public float climbStrength = 6f;
    public float glideFallSpeed = 0.5f;
    public float maxVerticalSpeed = 8f;
    public float dismountInputDelay = 0.3f;
    public float collisionDismountDelay = 1f;

    private Player nearbyPlayer;
    private Player mountedPlayer;
    private PlayerSkygliderState mountedPlayerState;
    private Rigidbody gliderRigidbody;
    private Collider gliderCollider;
    private float currentPitch;
    private Vector3 pendingDismountPosition;
    private bool hasPendingDismountPosition;
    private float mountTime;

    private void Awake()
    {
        Debug.Log("Skyglider -> Awake on " + name);
        gliderRigidbody = GetComponent<Rigidbody>();
        gliderCollider = GetComponent<Collider>();

        if (gliderRigidbody == null)
        {
            gliderRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        if (gliderCollider == null)
        {
            gliderCollider = gameObject.AddComponent<BoxCollider>();
        }

        gliderRigidbody.useGravity = false;
        gliderRigidbody.linearDamping = 1.5f;
        gliderRigidbody.angularDamping = 2f;
        gliderRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
        gliderRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gliderRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        // The main glider body must stay solid so it can collide with islands.
        gliderCollider.isTrigger = false;

        if (seatPoint == null)
        {
            seatPoint = transform;
            Debug.Log("Skyglider -> SeatPoint not assigned, defaulting to transform.");
        }

        SetParkedState();
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

        if (mountedPlayer.InteractAction != null &&
            mountedPlayer.InteractAction.triggered &&
            Time.time >= mountTime + dismountInputDelay)
        {
            Debug.Log("Skyglider -> interact pressed while mounted. Manual dismount requested.");
            DismountPlayer("manual interact");
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
        mountTime = Time.time;
        SetFlightState();

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

        Debug.Log("Skyglider -> Player mounted successfully. Player=" + player.name + ", SeatPoint=" + seatPoint.name);
    }

    private void DismountPlayer(string reason)
    {
        if (mountedPlayer == null)
        {
            Debug.Log("Skyglider -> DismountPlayer called but mountedPlayer is null.");
            return;
        }

        Player player = mountedPlayer;
        PlayerSkygliderState skygliderState = mountedPlayerState;

        player.transform.SetParent(null);
        if (hasPendingDismountPosition)
        {
            player.transform.position = pendingDismountPosition;
            Debug.Log("Skyglider -> Dismount position snapped to collision contact point: " + pendingDismountPosition);
        }
        else
        {
            player.transform.position = transform.position + transform.TransformDirection(dismountOffset);
            Debug.Log("Skyglider -> Dismount position used fallback offset: " + player.transform.position);
        }

        Rigidbody playerRigidbody = GetPlayerRigidbody(player);
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            playerRigidbody.useGravity = true;
            playerRigidbody.linearVelocity = hasPendingDismountPosition ? Vector3.zero : gliderRigidbody.linearVelocity;
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
        hasPendingDismountPosition = false;
        SetParkedState();

        Debug.Log("Skyglider -> Player dismounted. Reason: " + reason);
    }

    private void HandleFlight()
    {
        Vector2 moveInput = mountedPlayer.moveAction != null
            ? mountedPlayer.moveAction.ReadValue<Vector2>()
            : Vector2.zero;

        Vector2 lookInput = mountedPlayer.orientationAction != null
            ? mountedPlayer.orientationAction.ReadValue<Vector2>()
            : Vector2.zero;

        float yawInput = moveInput.x;
        float pitchInput = Mathf.Abs(lookInput.y) > 0.01f ? -lookInput.y : moveInput.y;

        Quaternion yawRotation = Quaternion.Euler(0f, yawInput * turnSpeed * Time.fixedDeltaTime, 0f);
        gliderRigidbody.MoveRotation(gliderRigidbody.rotation * yawRotation);

        currentPitch += pitchInput * pitchSpeed * Time.fixedDeltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -20f, 20f);

        Vector3 currentEuler = gliderRigidbody.rotation.eulerAngles;
        Quaternion flightRotation = Quaternion.Euler(currentPitch, currentEuler.y, 0f);
        gliderRigidbody.MoveRotation(flightRotation);

        float climbLift = (currentPitch / 20f) * climbStrength;
        float verticalSpeed = Mathf.Clamp(-glideFallSpeed + climbLift, -maxVerticalSpeed, maxVerticalSpeed);

        Vector3 forwardVelocity = transform.forward * cruiseSpeed;
        Vector3 targetVelocity = new Vector3(forwardVelocity.x, verticalSpeed, forwardVelocity.z);

        gliderRigidbody.linearVelocity = targetVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mountedPlayer == null)
            return;

        if (Time.time < mountTime + collisionDismountDelay)
        {
            Debug.Log("Skyglider -> ignoring collision auto-dismount during launch grace period with " + collision.collider.name);
            return;
        }

        if (collision.collider != null && !collision.collider.isTrigger)
        {
            if (ShouldIgnoreAutoDismount(collision.collider))
            {
                Debug.Log("Skyglider -> ignoring auto-dismount collision with " + collision.collider.name);
                return;
            }

            CacheGroundDismountPosition(collision);
            Debug.Log("Skyglider -> collided with " + collision.collider.name + ". Auto-dismounting player.");
            DismountPlayer("collision with " + collision.collider.name);
        }
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

    private void SetParkedState()
    {
        gliderRigidbody.linearVelocity = Vector3.zero;
        gliderRigidbody.angularVelocity = Vector3.zero;
        gliderRigidbody.isKinematic = true;
    }

    private void SetFlightState()
    {
        gliderRigidbody.isKinematic = false;
        gliderRigidbody.linearVelocity = Vector3.zero;
        gliderRigidbody.angularVelocity = Vector3.zero;
    }

    private bool ShouldIgnoreAutoDismount(Collider otherCollider)
    {
        if (ignoredAutoDismountColliders == null || otherCollider == null)
            return false;

        for (int i = 0; i < ignoredAutoDismountColliders.Length; i++)
        {
            Collider ignoredCollider = ignoredAutoDismountColliders[i];
            if (ignoredCollider != null && ignoredCollider == otherCollider)
            {
                return true;
            }
        }

        return false;
    }

    private void CacheGroundDismountPosition(Collision collision)
    {
        hasPendingDismountPosition = false;

        if (collision.contactCount <= 0)
        {
            Debug.Log("Skyglider -> collision had no contact points. Cannot cache landing point.");
            return;
        }

        ContactPoint contact = collision.GetContact(0);
        pendingDismountPosition = contact.point + (contact.normal * 1.25f);
        hasPendingDismountPosition = true;
        Debug.Log("Skyglider -> cached collision contact point from " + collision.collider.name +
                  " at " + contact.point + " with normal " + contact.normal);
    }
}
