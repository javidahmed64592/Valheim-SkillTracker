using System.Collections.Generic;
using System.Globalization;

using SkillTracker.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SkillTracker.Tracker;

/// <summary>
/// Shows each skill you earn experience in below the minimap: its icon, name, level and progress towards the next
/// level, with the experience of every gain rising and fading beside it. A skill's row fades out a few seconds after
/// its last gain, and up to <see cref="MaxRows"/> skills show at once.
/// </summary>
internal sealed class TrackerPanel : MonoBehaviour
{
    private const float RowWidth = 200f;
    private const float RowHeight = 34f;
    private const float RowGap = 4f;
    private const float IconSize = 28f;
    private const int MaxRows = 3;

    /// <summary>Just below vanilla's 200 × 200 minimap, which sits 40 px in from the top right corner.</summary>
    private static readonly Vector2 Position = new(-40f, -250f);

    private const float FadeTime = 1f;
    private const float LevelUpFlash = 2f;
    private const float DropLife = 1.5f;
    private const float DropRise = 30f;

    private static readonly Color PanelColor = new(0f, 0f, 0f, 0.45f);
    private static readonly Color TrackColor = new(0f, 0f, 0f, 0.6f);
    private static readonly Color BarColor = new(0.95f, 0.72f, 0.25f);
    private static readonly Color LevelUpColor = new(1f, 0.85f, 0.3f);

    private static TrackerPanel _instance;

    private readonly List<Row> _rows = [];
    private readonly List<Drop> _drops = [];
    private TMP_FontAsset _font;
    private Material _fontMaterial;

    public static void Create(Hud hud)
    {
        if (hud.m_rootObject == null || hud.m_healthText == null)
        {
            return;
        }

        GameObject root = new("SkillTracker", typeof(RectTransform));
        RectTransform rect = (RectTransform)root.transform;
        rect.SetParent(hud.m_rootObject.transform, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one;
        rect.anchoredPosition = Position;
        rect.sizeDelta = new Vector2(RowWidth, RowHeight);

        _instance = root.AddComponent<TrackerPanel>();
        _instance._font = hud.m_healthText.font;
        _instance._fontMaterial = hud.m_healthText.fontSharedMaterial;
    }

    /// <summary>Called after the local player's skill gains experience.</summary>
    public static void OnGain(Skills.Skill skill, float gained, bool leveledUp)
    {
        if (_instance != null && TrackerConfig.ShowSkillTracker.Value)
        {
            _instance.Show(skill, gained, leveledUp);
        }
    }

    private void Show(Skills.Skill skill, float gained, bool leveledUp)
    {
        Row row = _rows.Find(r => r.Skill == skill.m_info.m_skill);
        if (row == null)
        {
            if (_rows.Count >= MaxRows)
            {
                RemoveRow(Oldest());
            }
            row = CreateRow(skill);
            _rows.Add(row);
            Layout();
        }

        row.LastGain = Time.time;
        row.Group.alpha = 1f;
        row.Level.text = ((int)skill.m_level).ToString(CultureInfo.InvariantCulture);
        row.Fill.anchorMax = new Vector2(skill.m_level >= 100f ? 1f : skill.GetLevelPercentage(), 1f);
        if (leveledUp)
        {
            row.FlashUntil = Time.time + LevelUpFlash;
        }

        if (gained > 0f && TrackerConfig.ShowXpDrops.Value)
        {
            AddDrop(row, gained);
        }
    }

    private void Update()
    {
        float now = Time.time;
        float delay = TrackerConfig.TrackerFadeDelay.Value;
        for (int i = _rows.Count - 1; i >= 0; i--)
        {
            Row row = _rows[i];
            float fade = (now - row.LastGain - delay) / FadeTime;
            if (fade >= 1f)
            {
                RemoveRow(row);
                continue;
            }
            row.Group.alpha = 1f - Mathf.Clamp01(fade);
            row.Level.color = now < row.FlashUntil ? LevelUpColor : Color.white;
        }

        for (int i = _drops.Count - 1; i >= 0; i--)
        {
            Drop drop = _drops[i];
            float t = (now - drop.Start) / DropLife;
            if (t >= 1f)
            {
                Destroy(drop.Text.gameObject);
                _drops.RemoveAt(i);
                continue;
            }
            drop.Rect.anchoredPosition = new Vector2(drop.Rect.anchoredPosition.x, drop.StartY + (DropRise * t));
            drop.Text.alpha = 1f - (t * t);
        }
    }

    private Row Oldest()
    {
        Row oldest = _rows[0];
        foreach (Row row in _rows)
        {
            if (row.LastGain < oldest.LastGain)
            {
                oldest = row;
            }
        }
        return oldest;
    }

    private void RemoveRow(Row row)
    {
        _ = _rows.Remove(row);
        Destroy(row.Root);
        Layout();
    }

    private void Layout()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            _rows[i].Rect.anchoredPosition = new Vector2(0f, -i * (RowHeight + RowGap));
        }
    }

    private Row CreateRow(Skills.Skill skill)
    {
        GameObject root = new("Row", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        RectTransform rect = (RectTransform)root.transform;
        rect.SetParent(transform, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one;
        rect.sizeDelta = new Vector2(RowWidth, RowHeight);
        Image panel = root.GetComponent<Image>();
        panel.color = PanelColor;
        panel.raycastTarget = false;
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        Image icon = CreateImage(rect, "Icon", Color.white);
        icon.sprite = skill.m_info.m_icon;
        icon.preserveAspect = true;
        RectTransform iconRect = icon.rectTransform;
        iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(3f, 0f);
        iconRect.sizeDelta = new Vector2(IconSize, IconSize);

        const float left = IconSize + 8f;
        TextMeshProUGUI name = CreateText(rect, "Name", 15f, TextAlignmentOptions.MidlineLeft);
        name.text = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLowerInvariant());
        Stretch(name.rectTransform, left, 46f, 2f, 14f);

        TextMeshProUGUI level = CreateText(rect, "Level", 15f, TextAlignmentOptions.MidlineRight);
        Stretch(level.rectTransform, RowWidth - 46f, 6f, 2f, 14f);

        Image track = CreateImage(rect, "Track", TrackColor);
        Stretch(track.rectTransform, left, 6f, RowHeight - 11f, 5f);
        Image fill = CreateImage(track.rectTransform, "Fill", BarColor);
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;

        return new Row
        {
            Skill = skill.m_info.m_skill,
            Root = root,
            Rect = rect,
            Group = group,
            Level = level,
            Fill = fillRect,
        };
    }

    private void AddDrop(Row row, float gained)
    {
        TextMeshProUGUI text = CreateText(transform, "XpDrop", 16f, TextAlignmentOptions.MidlineRight);
        text.text = "+" + gained.ToString(DropFormat(gained), CultureInfo.InvariantCulture);
        text.color = BarColor;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(1f, 0.5f);
        rect.sizeDelta = new Vector2(80f, 20f);
        float y = row.Rect.anchoredPosition.y - (RowHeight / 2f);
        rect.anchoredPosition = new Vector2(-RowWidth - 6f, y);
        _drops.Add(new Drop { Text = text, Rect = rect, Start = Time.time, StartY = y });
    }

    /// <summary>Two significant figures for small gains such as dodges, whole numbers for big ones.</summary>
    private static string DropFormat(float gained)
    {
        return gained switch
        {
            >= 10f => "0",
            >= 1f => "0.#",
            _ => "0.##",
        };
    }

    private TextMeshProUGUI CreateText(Transform parent, string name, float size, TextAlignmentOptions alignment)
    {
        GameObject go = new(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = _font;
        text.fontSharedMaterial = _fontMaterial;
        text.fontSize = size;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static Image CreateImage(Transform parent, string name, Color color)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    /// <summary>Places a child by its distance from the parent's left, right and top edges, with a fixed height.</summary>
    private static void Stretch(RectTransform rect, float left, float right, float top, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(left, -top - height);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private sealed class Row
    {
        public Skills.SkillType Skill;
        public GameObject Root;
        public RectTransform Rect;
        public CanvasGroup Group;
        public TextMeshProUGUI Level;
        public RectTransform Fill;
        public float LastGain;
        public float FlashUntil;
    }

    private sealed class Drop
    {
        public TextMeshProUGUI Text;
        public RectTransform Rect;
        public float Start;
        public float StartY;
    }
}
