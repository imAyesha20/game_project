using GameProject.Models;
using GameProject.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameProject.UI
{
    // The UI displays information and collects input.
    public class GameUI
    {
        private readonly GameService _service;
        public GameUI(GameService service) { _service = service; }

        public void Start()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                ShowTitle();
                Console.WriteLine("1. Start game");
                Console.WriteLine("2. View scores");
                Console.WriteLine("3. How to play");
                Console.WriteLine("4. Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                if (choice == "1") PlayGame();
                else if (choice == "2") ShowScores();
                else if (choice == "3") ShowInstructions();
                else if (choice == "4") running = false;
                else { Console.WriteLine("Invalid option. Press Enter."); Console.ReadLine(); }
            }
        }

        private void PlayGame()
        {
            Console.Clear();
            Console.Write("Player name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = "Player";

            Player player = _service.CreatePlayer(name);
            List<Monster> monsters = _service.CreateMonsters();

            foreach (Monster monster in monsters)
            {
                Console.WriteLine("\nA " + monster.Name + " appears!");
                RunBattle(player, monster);
                if (!player.IsAlive()) break;
                _service.GiveMonsterReward(player, monster);
                Console.WriteLine(monster.Name + " defeated! Score: " + player.Score);
            }

            bool won = player.IsAlive();
            if (won)
            {
                int bonus = _service.CalculateVictoryBonus(player);
                player.AddScore(bonus);
                Console.WriteLine("\nYou defeated every monster! Bonus: " + bonus);
            }
            else Console.WriteLine("\nYou were defeated.");

            _service.SaveResult(player, won);
            Console.WriteLine("Final score: " + player.Score);
            Pause();
        }

        private void RunBattle(Player player, Monster monster)
        {
            while (player.IsAlive() && monster.IsAlive())
            {
                ShowBattleStatus(player, monster);
                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Use healing potion");
                Console.Write("Action: ");
                string action = Console.ReadLine();

                if (action == "1")
                    Console.WriteLine("You dealt " + _service.PlayerAttack(player, monster) + " damage.");
                else if (action == "2")
                    Console.WriteLine(player.UseHealingPotion() ? "Health restored." : "No potions left.");
                else
                    Console.WriteLine("Invalid action. You lose this turn.");

                if (monster.IsAlive())
                    Console.WriteLine(monster.Name + " dealt " + _service.MonsterAttack(monster, player) + " damage.");
            }
        }

        private void ShowBattleStatus(Player player, Monster monster)
        {
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine(player.Name + " HP: " + player.Health + " | Potions: " + player.HealingPotions);
            Console.WriteLine(monster.Name + " HP: " + monster.Health);
            Console.WriteLine("-----------------------------");
        }

        private void ShowScores()
        {
            Console.Clear();
            Console.WriteLine("=== SAVED SCORES ===");
            // GetTopScores is an advanced student exercise in the service layer.
            List<GameResult> scores = _service.GetTopScores(5);
            if (scores.Count == 0) Console.WriteLine("No saved games yet.");
            foreach (GameResult score in scores)
                Console.WriteLine(score.PlayerName + " - " + score.Score + " - " + score.Result);
            Pause();
        }

        private void ShowTitle()
        {
            Console.WriteLine("=============================");
            Console.WriteLine("     MONSTER BATTLE ARENA");
            Console.WriteLine("=============================");
        }


        private void ShowInstructions()
        {
            Console.Clear();

            Console.WriteLine("========== HOW TO PLAY ==========");
            Console.WriteLine();
            Console.WriteLine("1. You will fight different monsters.");
            Console.WriteLine("2. Use attack option to attack the monster.");
            Console.WriteLine("3. Use healing potions to restore your health.");
            Console.WriteLine("4. Defeat all monsters to win the game.");
            Console.WriteLine("5. If your health becomes zero, you lose.");
            Console.WriteLine("6. Defeated monsters award points.");
            Console.WriteLine("7. Winning the game may give you bonus points.");
            Console.WriteLine("8. Remaining health and unused potions");
            Console.WriteLine("   contribute to your victory bonus.");

            Console.WriteLine();
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("\nPress Enter to return to the menu.");
            Console.ReadLine();
        }
    }
}
