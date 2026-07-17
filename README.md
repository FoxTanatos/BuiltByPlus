# BuiltBy+

Torch plugin for Space Engineers servers.

BuiltBy is the game mechanic that stores who originally built a block.  
This is separate from block ownership.

The plugin fixes MES-spawned NPC grids where blocks have no proper BuiltBy value.  
It assigns BuiltBy to the NPC faction founder and enables the NPC claim button, similar to Prototech / The Factorum structures.

After a player claims the NPC grid, the blocks are transferred correctly and count toward the player's PCU instead of being treated as free blocks.

## Temporary Astronaut cleanup

The plugin also includes a temporary cleanup tool for duplicated NPC Astronaut identities.

It removes old duplicated offline Astronaut identity records from the save and keeps the active Astronaut identities used by the game.

This was added as a temporary workaround for servers affected by duplicated Astronaut records.
