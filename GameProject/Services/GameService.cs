using GameProject.Models;
using GameProject.Repositories;
using System;
using System.Collections.Generic;

namespace GameProject.Services
{
    // The service contains rules and calculations, but no Console code.
    public class GameService
    {
        private readonly ScoreRepository _repository;
        private readonly Random _random = new Random();

        public GameService(ScoreRepository repository) { _repository = repository; }
        public Player CreatePlayer(string name) { return new Player(name); }


        public List<Monster> CreateMonsters()
        {
            List<Monster> monsters = new List<Monster>();
            monsters.Add(new Monster("Slime", 40, 8, 10));
            monsters.Add(new Monster("Goblin", 65, 12, 20));
            monsters.Add(new Monster("Dragon", 100, 16, 40));
            return monsters;
        }

        public int PlayerAttack(Player player, Monster monster)
        {
            int damage = CalculateDamage(player.AttackPower);
            monster.TakeDamage(damage);
            return damage;
        }

        public int MonsterAttack(Monster monster, Player player)
        {
            int damage = CalculateDamage(monster.AttackPower);
            player.TakeDamage(damage);
            return damage;
        }

        private int CalculateDamage(int attackPower)
        {
            return _random.Next(attackPower - 3, attackPower + 4);
        }

        public void GiveMonsterReward(Player player, Monster monster)
        {
            player.AddScore(monster.ScoreValue);
        }

        public int CalculateVictoryBonus(Player player)
        {

            return 0;
        }

        public void SaveResult(Player player, bool won)
        {
            string result = won ? "Won" : "Lost";
            _repository.Save(new GameResult(player.Name, player.Score, result));
        }

        public List<GameResult> GetScores() { return _repository.GetAll(); }

        public List<GameResult> GetTopScores(int numberOfScores)
        
        {
            List<GameResult> allScores = _repository.GetAll();

            List<GameResult> sortedScores = new List<GameResult>();

            // Copy all results into a new list
            for (int i = 0; i < allScores.Count; i++)
            {
                sortedScores.Add(allScores[i]);
            }

            // Sort scores from highest to lowest
            for (int i = 0; i < sortedScores.Count; i++)
            {
                for (int j = i + 1; j < sortedScores.Count; j++)
                {
                    bool shouldSwap = false;

                    if (sortedScores[j].Score > sortedScores[i].Score)
                    {
                        shouldSwap = true;
                    }
                    else if (sortedScores[j].Score == sortedScores[i].Score)
                    {
                        if (string.Compare(
                            sortedScores[j].PlayerName,
                            sortedScores[i].PlayerName,
                            true) < 0)
                        {
                            shouldSwap = true;
                        }
                    }

                    if (shouldSwap)
                    {
                        GameResult temp = sortedScores[i];

                        sortedScores[i] = sortedScores[j];

                        sortedScores[j] = temp;
                    }
                }
            }

            // Create final list
            List<GameResult> topScores = new List<GameResult>();

            for (int i = 0; i < sortedScores.Count && i < numberOfScores; i++)
            {
                topScores.Add(sortedScores[i]);
            }

            return topScores;
        }
          
        }
    }

