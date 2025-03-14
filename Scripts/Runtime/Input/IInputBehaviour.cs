﻿namespace HatchStudios.Input
{
    public enum InputEnableMode : byte
    {
        BasedOnContext,
        AlwaysEnabled,
        AlwaysDisabled,
        Manual
    }
    public interface IInputBehaviour
    {
        bool Enabled { get; set; }
        InputEnableMode EnableMode { get; }
    }
}