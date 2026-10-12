using UnityEngine;

// AI 작성 가상 시연, 실제 학생 아님
// Written from scratch by Codex AI for a documentation fixture.
// demo-2026-a and demo-2026-b are fictional role labels, not code authors.
// Unity installation, compilation, Play Mode, and builds have NOT been verified.
public sealed class TinyBeaconGame : MonoBehaviour
{
    private enum RoundState
    {
        Ready,
        Playing,
        Won
    }

    private const int TargetCount = 3;
    private const float PanelWidth = 640f;
    private const float PanelHeight = 540f;

    private RoundState state = RoundState.Ready;
    private int completedCount;
    private string feedback = "Press Start, then tap 1 -> 2 -> 3.";

    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle feedbackStyle;
    private GUIStyle beaconStyle;

    // Runtime-only object creation: this does not create or save a Scene asset.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForEmptyScene()
    {
        if (Object.FindFirstObjectByType<TinyBeaconGame>() != null)
        {
            return;
        }

        new GameObject("Tiny Beacon Tap (AI Demo)").AddComponent<TinyBeaconGame>();
    }

    private void Awake()
    {
        ResetRound();
    }

    private void StartRound()
    {
        if (state != RoundState.Ready)
        {
            return;
        }

        completedCount = 0;
        state = RoundState.Playing;
        feedback = "Tap beacon 1 first. Wrong targets do not add points.";
    }

    private void ResetRound()
    {
        completedCount = 0;
        state = RoundState.Ready;
        feedback = "Press Start, then tap 1 -> 2 -> 3.";
    }

    private void TapBeacon(int index)
    {
        if (state != RoundState.Playing)
        {
            return;
        }

        // The next valid index equals the number of targets already completed.
        // Repeated or out-of-order clicks leave both state and count unchanged.
        if (index != completedCount)
        {
            feedback = "Wrong order. Count unchanged. Next beacon: "
                + (completedCount + 1) + ".";
            return;
        }

        completedCount++;
        if (completedCount == TargetCount)
        {
            state = RoundState.Won;
            feedback = "All three beacons reached! Restart returns to Ready.";
            return;
        }

        feedback = "Correct! Next beacon: " + (completedCount + 1) + ".";
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            alignment = TextAnchor.MiddleCenter
        };
        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        feedbackStyle = new GUIStyle(bodyStyle)
        {
            fontSize = 16
        };
        beaconStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 23
        };
    }

    private void OnGUI()
    {
        EnsureStyles();
        Matrix4x4 previousMatrix = GUI.matrix;
        bool previousEnabled = GUI.enabled;
        float scale = Mathf.Min(Screen.width / PanelWidth, Screen.height / PanelHeight);
        if (scale <= 0f)
        {
            return;
        }

        GUI.matrix = Matrix4x4.TRS(
            new Vector3(
                (Screen.width - PanelWidth * scale) * 0.5f,
                (Screen.height - PanelHeight * scale) * 0.5f,
                0f),
            Quaternion.identity,
            new Vector3(scale, scale, 1f));

        try
        {
            GUI.Box(new Rect(0, 0, PanelWidth, PanelHeight), GUIContent.none);
            GUI.Label(new Rect(20, 16, 600, 44), "Tiny Beacon Tap", titleStyle);
            GUI.Label(new Rect(20, 64, 600, 28), "AI-authored fictional demo fixture", bodyStyle);

            string next = state == RoundState.Playing ? (completedCount + 1).ToString() : "-";
            GUI.Label(new Rect(20, 101, 600, 32),
                "State: " + state + "   |   Count: " + completedCount + "/3   |   Next: " + next,
                bodyStyle);
            GUI.Label(new Rect(20, 137, 600, 34), "Click in order: upper left -> upper right -> lower center", bodyStyle);

            GUI.enabled = previousEnabled && state == RoundState.Playing;
            DrawBeacon(new Rect(70, 190, 170, 74), 0);
            DrawBeacon(new Rect(400, 190, 170, 74), 1);
            DrawBeacon(new Rect(235, 310, 170, 74), 2);

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(30, 399, 580, 50), feedback, feedbackStyle);

            GUI.enabled = previousEnabled && state == RoundState.Ready;
            if (GUI.Button(new Rect(140, 465, 170, 48), "Start", beaconStyle))
            {
                StartRound();
            }

            GUI.enabled = previousEnabled;
            if (GUI.Button(new Rect(330, 465, 170, 48), "Restart", beaconStyle))
            {
                ResetRound();
            }
        }
        finally
        {
            GUI.enabled = previousEnabled;
            GUI.matrix = previousMatrix;
        }
    }

    private void DrawBeacon(Rect bounds, int index)
    {
        string label = (index + 1).ToString();
        if (index < completedCount)
        {
            label += "  OK";
        }

        if (GUI.Button(bounds, label, beaconStyle))
        {
            TapBeacon(index);
        }
    }
}
