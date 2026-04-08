using System;
using Godot;

public partial class GeneralDescriptionWithSubtitleUI : GeneralDescriptionUI
{
    protected RichTextLabel subtitleLabel = null!;

    public override void _Ready()
    {
        base._Ready();
        subtitleLabel = GetNode<RichTextLabel>("%SubtitleLabel");
    }

    public void SetSubtitle(string subtitle)
    {
        subtitleLabel.Text = Style.SubTitle(subtitle);
    }

    public override void Setup(Node target)
    {
        base.Setup(target);

        switch (target)
        {
            case HiveGridObject hive:
                Action setBeeCount = () =>
                {
                    SetSubtitle(
                        Style.SubTitle($"{hive.BeeCount}/{GameStore.HiveCapacityBee.Value} bees")
                    );
                };
                HiveGridObject.BeeAddedEventHandler onBeeAdded = (Bee bee) => setBeeCount();
                hive.BeeAdded += onBeeAdded;
                TreeExiting += () => hive.BeeAdded -= onBeeAdded;
                setBeeCount();
                break;

            case Flower flower:
                void setHoney()
                {
                    if (flower.CurState == Flower.State.Pollinating)
                        SetSubtitle(
                            Style.SubTitle(
                                $"pollinating ({(flower.Honey / flower.HoneyCost.Value) * 100f:0}%)"
                            )
                        );
                    else
                        SetSubtitle(Style.SubTitle($"{flower.Honey} honey"));
                }
                Flower.HoneyChangedEventHandler onHoneyChanged = (_) => setHoney();
                flower.HoneyChanged += onHoneyChanged;
                Flower.EmptiedEventHandler onEmptied = () => setHoney();
                flower.Emptied += onEmptied;
                Flower.PollinatedEventHandler onPollinated = (_) => setHoney();
                flower.Pollinated += onPollinated;
                TreeExiting += () =>
                {
                    flower.HoneyChanged -= onHoneyChanged;
                    flower.Emptied -= onEmptied;
                    flower.Pollinated -= onPollinated;
                };
                setHoney();
                break;
        }
    }
}
