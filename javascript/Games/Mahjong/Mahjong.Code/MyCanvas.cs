﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.CSharp.Extensions;
using Mahjong.Shared;
using System.Windows.Media;
using System.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows;
using ScriptCoreLib.Shared.Avalon.TextSuggestions;

namespace Mahjong.Code
{
	[Script]
	public class MyCanvas : Canvas
	{
		// http://cynagames.com/

		public const double DefaultScale = 1;

		public const int DefaultWidth = 600;
		public const int DefaultHeight = 500;

		public const int DefaultScaledWidth = (int)((double)DefaultWidth * DefaultScale);
		public const int DefaultScaledHeight = (int)((double)DefaultHeight * DefaultScale);

		// Set if the platform supports playing embeddded sounds
		// WPF does not (2008 09)

		public readonly FutureAction<string> PlaySoundFuture;
		public readonly Action<string> PlaySound;

		public MyCanvas()
		{
			this.PlaySoundFuture = new FutureAction<string>();
			this.PlaySound = this.PlaySoundFuture;

			this.Width = DefaultScaledWidth;
			this.Height = DefaultScaledHeight;

			// http://www.partnersinrhyme.com/pir/free_music_loops.shtml
			// http://www.soundsnap.com/taxonomy/term/62

			var Sounds = new
			{
				click = PlaySoundFuture["click"],
				birds = PlaySoundFuture["birds"],
				flag = PlaySoundFuture["flag"],
				buzzer = PlaySoundFuture["buzzer"],
				reveal = PlaySoundFuture["reveal"]
			};

			Sounds.birds();

			#region background
			new Image
			{
				Source = "assets/Mahjong.Assets/china.jpg".ToSource(),
				Stretch = System.Windows.Media.Stretch.Fill,
				Width = DefaultScaledWidth,
				Height = DefaultScaledHeight
			}.AttachTo(this);

			new Image
			{
				Source = "assets/Mahjong.Assets/shadow.png".ToSource(),
				Stretch = System.Windows.Media.Stretch.Fill,
				Width = DefaultScaledWidth,
				Height = DefaultScaledHeight
			}.AttachTo(this);
			#endregion

			var MyLayout = new VisibleLayout(
				new VisibleLayout.SettingsInfo
				{
					Scale = DefaultScale,
					ScaledWidth = DefaultScaledWidth,
					ScaledHeight = DefaultScaledHeight,
					CreateOverlay = true,
				}
			);


			
			MyLayout.Container.AttachTo(this);

			#region CoPlayer mouse
			// place stuff between tiles and cursor here
			var CoPlayer2 = new Image
			{
				Source = CursorAssets.Cursors["red"].ToSource()
			}.MoveTo(32, 32).AttachTo(this);


			(1000 / 30).AtIntervalWithCounter(
				Counter =>
				{
					CoPlayer2.MoveTo(
						DefaultScaledWidth / 2 + Math.Cos((double)Counter / 20) * 64,
						DefaultScaledHeight / 2 + Math.Sin((double)Counter / 20) * 64
					);
				}
			);
			#endregion

			//MyLayout.Overlay.Opacity = 0.1;

			MyLayout.Overlay.AttachTo(this);

			#region Layout Selection
			// http://www.stripegenerator.com
			var Stripes = new Image
			{
				Source = "assets/Mahjong.Assets/stripes.png".ToSource(),
				Stretch = System.Windows.Media.Stretch.Fill,
				Width = DefaultScaledWidth,
				Height = DefaultScaledHeight,
				Visibility = Visibility.Hidden
			}.AttachTo(this);


			var CommentMargin = 8;
			var CommentForUnfocusing = new TextBox
			{
				Width = DefaultScaledWidth - CommentMargin * 2,
				Height = 24,
				Background = Brushes.Transparent,
				Foreground = Brushes.White,
				BorderThickness = new Thickness(0),
				Text = "Mahjong Multiplayer",
				TextAlignment = TextAlignment.Left,
				IsReadOnly = true,
			}.MoveTo(CommentMargin, CommentMargin).AttachTo(this);



			var Comment = new TextBox
			{
				Width = DefaultScaledWidth - CommentMargin * 2,
				Height = 24,
				Background = Brushes.Transparent,
				Foreground = Brushes.White,
				BorderThickness = new Thickness(0),
				Text = "Loading...",
				TextAlignment = TextAlignment.Right,
				IsReadOnly = true,
			}.MoveTo(CommentMargin, CommentMargin).AttachTo(this);

			var CommentSuggestions = new TextSuggestionsControl(Comment, 6, CommentForUnfocusing, this)
			{
				Suggestions = new string[0],
				InactiveResultBackground = Brushes.Transparent,
				Margin = 2,
				Enabled = false
			};

			CommentSuggestions.Activate +=
				(bg, text) =>
				{
					bg.Opacity = 0.6;
				};

			CommentSuggestions.Enter +=
				delegate
				{
					Comment.Foreground = Brushes.Black;
					CommentForUnfocusing.Foreground = Brushes.Gray;
					Stripes.Visibility = Visibility.Visible;
				};

			CommentSuggestions.Exit +=
				delegate
				{
					Comment.Foreground = Brushes.White;
					CommentForUnfocusing.Foreground = Brushes.White;
					Stripes.Visibility = Visibility.Hidden;

					// we need to reload a map now...
				};

			Stripes.MouseLeftButtonUp +=
				delegate
				{
					CommentForUnfocusing.Focus();
				};
			#endregion


			MyLayout.LayoutDestroyed +=
				delegate
				{
					Console.WriteLine("LayoutDestroyed ...");

					//Comment.IsReadOnly = true;
					//CommentSuggestions.Enabled = false;
				};

			MyLayout.LayoutChanging +=
				delegate
				{
					Console.WriteLine("LayoutChanging ..." + MyLayout.Layout.Source);

					Comment.Text = MyLayout.Layout.Comment;

					// this occurs only after current load is complete
					MyLayout.LayoutProgress.Continue(
						delegate
						{
							Console.WriteLine("Layout loaded! " + MyLayout.Layout.Source);
						}
					);
					// we should suffle the ranks

					var SelectedTile = default(VisibleTile);

					foreach (var v in MyLayout.Tiles)
					{
						// any image

						v.Tile.Continue(
							(VisibleTile tt) =>
							{
								// ApplyDiagnostics(tt);

								ApplyDiagnosticsForPairs(MyLayout, tt);


								tt.MouseEnterWhenLayoutLoaded +=
									delegate
									{
										// we need to lock this tile
										// otherwise two network clients might take the same
										// tile yet get out of sync score
										//Console.WriteLine("lock: " + tt.Entry.RankImage.ToString());
									};

								tt.ClickWhenLayoutLoaded +=
									delegate
									{
										//tt.YellowFilter.Opacity = 0;

										Console.WriteLine(tt.Entry.RankImage.ToString());

										if (tt.Entry.BlockingSiblings.Any())
										{
											Sounds.buzzer();
										}
										else
										{
											if (tt.IsPairable(SelectedTile))
											{
												Console.WriteLine("pairable: " + SelectedTile.Rank.ToString());

												MyLayout.Remove(SelectedTile, tt);
											

												SelectedTile = null;
											}
											else
											{
												SelectedTile = tt;
											}

											Sounds.click();
										}

									};

							}
						);
					}

				};

			// this occurs always
			MyLayout.LayoutChanged +=
				delegate
				{
					// done loading new layout

					Sounds.reveal();


					Layouts.AllLoaded.Continue(
						delegate
						{
							Comment.IsReadOnly = false;
							CommentSuggestions.Enabled = true;
						}
					);

				};

			//var button1 = new TextBox
			//{
			//    Foreground = Brushes.White,
			//    Background = Brushes.Transparent,
			//    Text = "Next Layout »",
			//    IsReadOnly = true,
			//    Width = 120,
			//    Height = 24,
			//    BorderThickness = new Thickness(0),
			//    TextAlignment = TextAlignment.Center
			//}.MoveTo(8, DefaultScaledHeight - 32).AttachTo(this);

			//var button1_overlay = new Rectangle
			//{
			//    Width = 120,
			//    Height = 24,
			//    Fill = Brushes.Blue,
			//    Opacity = 0.0,
			//    Cursor = Cursors.Hand
			//}.MoveTo(8, DefaultScaledHeight - 32).AttachTo(this);

			//button1_overlay.MouseEnter +=
			//    delegate
			//    {
			//        button1.Foreground = Brushes.Blue;
			//    };

			//button1_overlay.MouseLeave +=
			//    delegate
			//    {
			//        button1.Foreground = Brushes.White;
			//    };



			Layouts = new LayoutsFuture(Assets.Default.FileNames.Where(k => k.EndsWith(".lay")).Randomize().ToArray());

			Layouts.FirstLoaded.Continue(
				value =>
				{
					MyLayout.Layout = value;
				}
			);

			Layouts.AllLoaded.Continue(
				ByComment =>
				{
					CommentSuggestions.Suggestions = ByComment.Keys.ToArray();

					CommentSuggestions.Select +=
						NewLayoutComment =>
						{
							// Do not reload the same layout
							if (MyLayout.Layout != null)
								if (NewLayoutComment == MyLayout.Layout.Comment)
									return;

							if (Layouts.ByComment.ContainsKey(NewLayoutComment))
							{
								// new layout does indeed exist!
								MyLayout.Layout = Layouts.ByComment[NewLayoutComment];
							}
						};
				}
			);
		}

		private static void ApplyDiagnosticsForPairs(VisibleLayout MyLayout, VisibleTile tt)
		{
			tt.MouseEnterLeaveWhenLayoutLoaded +=
				delegate
				{
					tt.BlackFilter.Visibility = Visibility.Hidden;

					return delegate
					{
						tt.BlackFilter.Visibility = Visibility.Visible;
					};
				};

			tt.MouseEnterLeaveWhenLayoutLoaded +=
				delegate
				{
					if (tt.Entry.BlockingSiblings.Any())
					{


						var SetOpacity = tt.Entry.BlockingSiblings.Select(
							k =>
								new Action<double>(
									Opacity => k.Tile.Value.YellowFilter.Opacity = Opacity
								)
							).Combine();

						tt.RedFilter.Opacity = 0.5;
						SetOpacity(0.5);

						return delegate
						{
							tt.RedFilter.Opacity = 0;
							SetOpacity(0);
						};
					}
					else
					{
						var SetOpacity = MyLayout.Pairs.Single(k => k.Contains(tt.Entry)).Select(
							k =>
								new Action<double>(
									Opacity => k.Tile.Value.GreenFilter.Opacity = Opacity
								)
							).Combine();


						SetOpacity(0.5);

						return delegate
						{
							SetOpacity(0);
						};
					}
				};
		}

		private static void ApplyDiagnostics(VisibleTile tt)
		{
			tt.MouseEnter +=
				delegate
				{
					tt.GreenFilter.Opacity = 0.5;
					tt.BlackFilter.Visibility = Visibility.Hidden;
				};



			tt.MouseLeave +=
				delegate
				{
					tt.GreenFilter.Opacity = 0;
					tt.BlackFilter.Visibility = Visibility.Visible;
				};

			// while loading the k.Tile.Value is null for siblings

			tt.MouseEnterWhenLayoutLoaded +=
				delegate
				{
					tt.Entry.Siblings.ForEach(k => k.Tile.Value.YellowFilter.Opacity = 0.5);
					tt.Entry.BlockingSiblings.ForEach(k => k.Tile.Value.RedFilter.Opacity = 0.5);
				};

			tt.MouseLeaveWhenLayoutLoaded +=
				delegate
				{
					tt.Entry.Siblings.ForEach(k => k.Tile.Value.YellowFilter.Opacity = 0);
					tt.Entry.BlockingSiblings.ForEach(k => k.Tile.Value.RedFilter.Opacity = 0);
				};
		}


		public readonly LayoutsFuture Layouts;

	}
}
