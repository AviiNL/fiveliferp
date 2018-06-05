namespace FiveLife.NativeUI
{
    public class UIMenuListItemItem
    {
        public string Label { get; set; }
        public object Value { get; set; }

        public UIMenuListItemItem(object Value)
            : this(Value, "") { }

        public UIMenuListItemItem(object Value, string Label)
        {
            this.Value = Value;
            this.Label = Label;
        }
    }
}