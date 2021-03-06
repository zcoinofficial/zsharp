namespace Zsharp.LightweightIndexer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NBitcoin;

    sealed class BlockListener : IBlockListener
    {
        readonly BlockSynchronizer service;

        public BlockListener(BlockSynchronizer service)
        {
            this.service = service;
        }

        public async Task DiscardBlocksAsync(int start, CancellationToken cancellationToken = default)
        {
            this.service.Logger?.LogInformation("Re-organize occurred on node side, starting re-organize at our side.");

            var (blocks, height) = await this.service.Repository.GetLatestsBlocksAsync(1, cancellationToken);

            while (height >= start)
            {
                await this.RemoveLastBlockAsync(blocks.Single(), height);

                (blocks, height) = await this.service.Repository.GetLatestsBlocksAsync(1);
            }
        }

        public async Task<int> GetStartBlockAsync(CancellationToken cancellationToken = default)
        {
            var (blocks, height) = await this.service.Repository.GetLatestsBlocksAsync(1, cancellationToken);

            if (!blocks.Any())
            {
                this.service.Logger?.LogInformation(
                    "No any blocks at our side, starting synchronization from the begining.");
                return 0;
            }

            var next = height + 1;

            this.service.Logger?.LogInformation(
                "Our latest block is {Height}:{Hash}, starting synchronization from block {Next}.",
                height,
                blocks.Single().GetHash(),
                next);

            return next;
        }

        public async Task<int> ProcessBlockAsync(Block block, int height, CancellationToken cancellationToken = default)
        {
            var latest = await this.service.Repository.GetLatestsBlocksAsync(1, cancellationToken);

            // Make sure the block is expected one.
            if (!latest.Blocks.Any())
            {
                if (height != 0)
                {
                    return 0;
                }

                if (block.GetHash() != this.service.Network.GetGenesis().GetHash())
                {
                    throw new ArgumentException("The block is not a genesis block.", nameof(block));
                }
            }
            else
            {
                var next = latest.Highest + 1;

                if (height != next)
                {
                    return next;
                }

                if (block.Header.HashPrevBlock != latest.Blocks.Single().GetHash())
                {
                    // Our latest block is not what expected (e.g. chain already switched) so we need to reload it.
                    this.service.Logger?.LogInformation(
                        "A new block {Height}:{Hash} is depend on {Previous} but we did not have it.",
                        height,
                        block.GetHash(),
                        block.Header.HashPrevBlock);

                    await this.RemoveLastBlockAsync(latest.Blocks.Single(), latest.Highest, cancellationToken);

                    return latest.Highest;
                }
            }

            await this.AddBlockAsync(block, height, cancellationToken);

            return height + 1;
        }

        async Task AddBlockAsync(Block block, int height, CancellationToken cancellationToken)
        {
            this.service.Logger?.LogInformation("Adding block {Height}:{Hash}.", height, block.GetHash());

            await this.service.Repository.AddBlockAsync(block, height, cancellationToken);
            await this.service.RaiseBlockAdded(block, height);
        }

        async Task RemoveLastBlockAsync(Block block, int height, CancellationToken cancellationToken = default)
        {
            this.service.Logger?.LogInformation("Removing block {Height}:{Hash}.", height, block.GetHash());

            await this.service.RaiseBlockRemovingAsync(block, height, cancellationToken);
            await this.service.Repository.RemoveLastBlockAsync();
        }
    }
}
