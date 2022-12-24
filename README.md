# aokana_extra2ct  
  
General usage: `aokana_extra2ct <.dat file>`  
(Eg: `aokana_extra2ct evcg.dat`)  
  
Processing CG data to produce complete CGs:  
  
Non-H: `aokana_extra2ct evcg.dat vcg`  
H: `aokana_extra2ct adult.dat vcg`  
  
Processing sprite data to produce complete sprites:  
  
Non-H:  
`aokana_extra2ct sprites.dat all`  
or  
`aokana_extra2ct sprites.dat <three letter chara name prefix>` to process a specific character's sprites only  
(Eg: `aokana_extra2ct sprites.dat mis`)  
H:  
First extract adult.dat with `aokana_extra2ct adult.dat`  
and then do `aokana_extra2ct sprites.dat mis` or `aokana_extra2ct sprites.dat all` to process sprites including H sprites  
  
CGs will be written to `_out_cgs` and sprites will be written to `_out_sprites`  
  
If you're brave enough to process all sprites, make sure there's enough space (in the ballpark of 30 gigs at least, probably more),  
and either a powerful CPU or a lot of time or both  
  
*A good amount of code in this repo is copy/pasted from the game's assembly via dnspy; No guarantees of any proper functionality  
  
**Uses Magick.NET to produce complete CGs and sprites; By default, uses an OpenMP enabled variant that will hammer all CPU threads; Change as necessary
