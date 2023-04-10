﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using WraithMod.Content.BaseClasses;
using WraithMod.Content.Class;
using WraithMod.Content.Items.Weapons.Necronomicons.BookOfEir;
using Microsoft.Xna.Framework;
using WraithMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using WraithMod.Content.Items.Weapons.Necronomicons.BookOfSif;
using Terraria.Audio;

namespace WraithMod.Content.Items.Weapons.Necronomicons.BookOfFreyja
{
    public class BookOfFreyja : BaseScythe
    {
        public static int TimeUse;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Book of Eir I");
            //Tooltip.SetDefault("Summons a necronomicon of Sif that summons the power of nature to aid you, shooting dayblooms and other herbs at your mouse.\nBlood Sacrifice " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 30;
            LifeCost = 30;
            Item.width = 62;
            Item.height = 62;

            Item.useStyle = ItemUseStyleID.MowTheLawn;
            Item.useTime = 40;
            TimeUse = Item.useTime;
            Item.noUseGraphic = true;
            Item.useAnimation = 40;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 9;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<BookOfFreyjaProj>(); // ID of the projectiles the sword will shoot
            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        // This method gets called when firing your weapon/sword.

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<BlessedHealth>(), 1800);
            Projectile.NewProjectile(Projectile.GetSource_None(), player.Center, Vector2.Zero, ModContent.ProjectileType<EirCircles>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(Projectile.GetSource_None(), player.Center, Vector2.Zero, ModContent.ProjectileType<EirQuad>(), 0, 0, player.whoAmI);
            string DeathMessage = "";
            int decider = Main.rand.Next(4);
            if (decider == 0)
            {
                DeathMessage = " was consumed by darkness.";
            }
            else if (decider == 1)
            {
                DeathMessage = " couldn't handle the power.";
            }
            else if (decider == 2)
            {
                DeathMessage = " didn't watch their health bar.";
            }
            else if (decider == 3)
            {
                DeathMessage = " forgot to heal.";
            }
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + DeathMessage), LifeCost, 0, false, true, -1, false, 1000, 2, 0);
            player.immuneTime = 0;
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 35)
                .AddTile(ModContent.TileType<RitualTable>())
                .Register();
        }
    }
    public class BookOfFreyjaProj : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(64, 36);
            Projectile.penetrate = -1;
            Projectile.aiStyle = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int d = 0; d < 50; d++)
            {
                Dust yellow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, 0f, 0f, 150, default, 1.5f);
                yellow.noGravity = true;
            }
            for (int d = 0; d < 50; d++)
            {
                Dust green = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkCrystalShard, 0f, 0f, 150, default, 1.5f);
                green.noGravity = true;
            }
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (PlayerDead.Dead)
            {
                Projectile.Kill();
            }
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Zero;
            if (player.direction == 1)
            {
                Projectile.spriteDirection = 1;
                Projectile.position = new Vector2(player.position.X + 20, player.position.Y);
            }
            else if (player.direction == -1)
            {
                Projectile.spriteDirection = -1;
                Projectile.position = new Vector2(player.position.X - 60, player.position.Y);
            }
            if (player.HeldItem.type != ModContent.ItemType<BookOfFreyja>())
            {
                Projectile.Kill();
            }
            if (player.HeldItem.type == ModContent.ItemType<BookOfFreyja>())
            {
                Projectile.timeLeft += 10;
            }
            Timer++;
            if (Timer > BookOfFreyja.TimeUse * 2)
            {
                // Our timer has finished, do something here:
                // SoundEngine.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
                for (int d = 0; d < 10; d++)
                {
                    Dust yellow = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, 0f, 150, default, 1.5f);
                    yellow.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.MaxMana);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CharmPulse>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Timer = 0;
            }
        }
        // Some advanced drawing because the texture image isn't centered or symetrical
        // If you dont want to manually drawing you can use vanilla projectile rendering offsets
        // Here you can check it https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#horizontal-sprite-example
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/BookOfFreyja/BookOfFreyjaGlow");
            Color color = Color.Lerp(Color.Pink, Color.Magenta, 20f);
            Color color2 = Color.Lerp(Color.White, color, 20f);
            var bloom = color2;
            bloom.A = 0;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.3f, SpriteEffects.None, 0);
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = 20f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

            // If sprite is vertical
            // float offsetY = 20f;
            // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
    public class CharmPulse : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(128, 128);
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
            Projectile.alpha = 125;
            Projectile.aiStyle = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CharmPulseGlow>(), 0, 0, Projectile.owner);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Timer++;
            if (Timer >= 70)
            {
                Projectile.Kill();
            }
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        { 
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = 64f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

            // If sprite is vertical
            // float offsetY = 20f;
            // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Color drawColor = Color.White;
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
    public class CharmPulseGlow : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(128, 128);
            Projectile.penetrate = -1;
            Projectile.scale = 2.15f;
            Projectile.alpha = 125;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Timer++;
            if (Timer >= 70)
            {
                Projectile.Kill();
            }
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/BookOfFreyja/CharmPulseGlow");
            Color color = Color.Lerp(Color.Pink, Color.Magenta, 20f);
            Color color2 = Color.Lerp(Color.White, color, 20f);
            var bloom = color2;
            bloom.A = 0;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = 96f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

            // If sprite is vertical
            // float offsetY = 20f;
            // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Main.EntitySpriteDraw(bloomTex.Value,
                Projectile.Center - Main.screenPosition,
                sourceRectangle, bloom, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}
