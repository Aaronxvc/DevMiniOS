# Path: MiniModel/py/outloud_mini_lm.py
"""
This file implements a word-level bigram language model for generating
suggested code completions.

How it works:
1.  Training:  It counts word co-occurrences from a corpus of text.
2.  Sampling: Given a prefix (sequence of words), it samples the next word
    based on the conditional probabilities learned during training.  It walks
    the chain sampling one word at a time until max_tokens is reached.

When to use it:
*   For quick prototyping of DSL completion in resource-constrained
    environments.
*   As a baseline before moving to more complex models.

Limitations:
*   Bigrams are context-limited (only considers the previous word).
*   Does not handle out-of-vocabulary words gracefully.
*   Naive implementation not optimized for speed or memory.

Future improvements:
*   Implement smoothing to handle unseen bigrams.
*   Use a more sophisticated tokenizer (e.g., subword units).
*   Consider a tiny character-level Transformer (nano-GPT) for better context.
"""
import json
import random
import re
import argparse

class BigramModel:
    """
    A word-level bigram language model.
    """
    def __init__(self):
        self.bigram_counts = {}
        self.word_counts = {}
        self.cumulative_probs = {}

    def tokenize(self, text):
        """
        Tokenizes the input text, preserving quoted spans as single tokens.
        Example: 'open app="journal"'  ->  ['open', 'app="journal"']
        """
        return re.findall(r'\w+="[^"]*"|\w+', text) # Tokenize, keeping quotes

    def train(self, corpus_file):
        """
        Trains the bigram model from a corpus file.
        """
        with open(corpus_file, 'r') as f:
            for line in f:
                tokens = self.tokenize(line.strip().lower())
                if not tokens:
                    continue

                # Update word counts
                for token in tokens:
                    self.word_counts[token] = self.word_counts.get(token, 0) + 1

                # Update bigram counts
                for i in range(len(tokens) - 1):
                    bigram = (tokens[i], tokens[i+1])
                    self.bigram_counts[bigram] = self.bigram_counts.get(bigram, 0) + 1

        self.calculate_cumulative_probabilities()

    def calculate_cumulative_probabilities(self):
        """
        Calculates cumulative probabilities for sampling.
        """
        for word, following_words in self.get_following_words().items():
            total_count = sum(self.bigram_counts[(word, w)] for w in following_words)
            cumulative_probability = 0.0
            self.cumulative_probs[word] = []
            for w in following_words:
                cumulative_probability += self.bigram_counts[(word, w)] / total_count
                self.cumulative_probs[word].append((w, cumulative_probability))

    def get_following_words(self):
        """
        Returns a dictionary of words and their following words.
        """
        following_words = {}
        for bigram, count in self.bigram_counts.items():
            first_word, second_word = bigram
            if first_word not in following_words:
                following_words[first_word] = set()
            following_words[first_word].add(second_word)
        return following_words

    def sample(self, prefix, max_tokens=10, seed=None):
        """
        Samples a completion from the bigram model given a prefix.
        """
        if seed is not None:
            random.seed(seed)  # For deterministic tests

        tokens = self.tokenize(prefix.strip().lower())
        completion = tokens[:] # start with what's given

        for _ in range(max_tokens):
            last_word = completion[-1]
            if last_word not in self.cumulative_probs:
                break # Stop if we don't know what follows

            # Sample next word
            rand = random.random()
            for word, cumulative_probability in self.cumulative_probs[last_word]:
                if rand < cumulative_probability:
                    completion.append(word)
                    break
            else:
                break # Should rarely happen, but be safe

        return {
            "completion": " ".join(completion[len(tokens):]), # Only return completion
            "tokens": completion[len(tokens):]
        }

    def save_model(self, model_file):
        """
        Saves the trained model to a JSON file.
        """
        model_data = {
            "bigram_counts": self.bigram_counts,
            "word_counts": self.word_counts
        }
        with open(model_file, 'w') as f:
            json.dump(model_data, f)

    def load_model(self, model_file):
        """
        Loads a trained model from a JSON file.
        """
        try:
            with open(model_file, 'r') as f:
                model_data = json.load(f)
            self.bigram_counts = model_data["bigram_counts"]
            self.word_counts = model_data["word_counts"]
            self.calculate_cumulative_probabilities() # recalc for safety
        except FileNotFoundError:
            print(f"Model file not found: {model_file}")
            exit(1)
        except json.JSONDecodeError:
            print(f"Invalid JSON in model file: {model_file}")
            exit(1)

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Bigram language model for code completion.')
    subparsers = parser.add_subparsers(dest='command', help='Command to execute')

    # Train command
    train_parser = subparsers.add_parser('train', help='Train the model')
    train_parser.add_argument('--corpus', required=True, help='Path to the corpus file')
    train_parser.add_argument('--model', required=True, help='Path to save the trained model')

    # Sample command
    sample_parser = subparsers.add_parser('sample', help='Sample from the model')
    sample_parser.add_argument('--model', required=True, help='Path to the trained model')
    sample_parser.add_argument('--prefix', required=True, help='Prefix to start the completion')
    sample_parser.add_argument('--max_tokens', type=int, default=10, help='Maximum number of tokens to generate')
    sample_parser.add_argument('--seed', type=int, default=None, help='Random seed for deterministic sampling') # Optional seed

    args = parser.parse_args()

    model = BigramModel()

    if args.command == 'train':
        model.train(args.corpus)
        model.save_model(args.model)
        print(f"Model saved to {args.model}")
    elif args.command == 'sample':
        model.load_model(args.model)
        result = model.sample(args.prefix, args.max_tokens, args.seed)
        print(json.dumps(result))
    else:
        parser.print_help()