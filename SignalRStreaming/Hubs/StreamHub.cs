﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRStreaming.Hubs
{
    public class StreamHub : Hub
    {
        public ChannelReader<int> DelayCounter(int delay, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();
            _ = WriteItems(channel.Writer, 20, delay, cancellationToken);
            return channel.Reader;
        }
        private async Task WriteItems(ChannelWriter<int> writer, int count, int delay, CancellationToken cancellationToken)
        {
            try
            {
            for (var i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                //For every 5 items streamed, add twice the delay
                if (i % 5 == 0)
                    delay = delay * 2;
                await writer.WriteAsync(i);
                await Task.Delay(delay);
            }
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }
            writer.TryComplete();
        }
    }
}
