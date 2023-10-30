﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Dialogs;

/// <summary>
/// Class to display SnackBar Messages
/// </summary>
public static class SnackBarMsg
{
    #region Queue a message (default duration)
    /// <summary>
    /// Queues the message (default duration).
    /// </summary>
    /// <param name="message">The message.</param>
    public static void QueueMessage(string message)
    {
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Enqueue(message);
    }
    #endregion Queue a message (default duration)

    #region Queue a message and set duration
    /// <summary>
    /// Queues the message with duration.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void QueueMessage(string message, int duration)
    {
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Enqueue(message,
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromMilliseconds(duration));
    }
    #endregion Queue a message and set duration

    #region Clear message queue then queue a message (default duration)
    /// <summary>
    /// Clears the message queue then queues a message (default duration).
    /// </summary>
    /// <param name="message">The message.</param>
    public static void ClearAndQueueMessage(string message)
    {
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Clear();
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Enqueue(message);
    }
    #endregion Clear message queue then queue a message (default duration)

    #region Clear message queue then queue a message and set duration
    /// <summary>
    /// Clears the message queue then queues a message with duration.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public static void ClearAndQueueMessage(string message, int duration)
    {
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Clear();
        (Application.Current.MainWindow as MainWindow)?.SnackBar1.MessageQueue.Enqueue(message,
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromMilliseconds(duration));
    }
    #endregion Clear message queue then queue a message and set duration
}
