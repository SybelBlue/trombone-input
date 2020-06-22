{-# LANGUAGE LambdaCase #-}

import Data.List (intercalate)
import Data.Char (toUpper)

-- | text prompts in the main scene
type Prompt = String

-- | commands that are entered into the file for Unity to perform
data Command = RandomizeLayoutOrder | NextLayout | TrialNumber Int deriving Show

{- |
    Challenges are individual items in the Trial file that Unity will read
    - Blind is the challenge where users can not backspace or view what they have typed
    - Perfect is the challenge where users must type the full prompt exactly
    - Practice is an untimed and unscored challenge that users may skip
-}
data Challenge = Blind Prompt | Perfect Prompt | Practice Prompt deriving Show

-- | challenges and commands that are written into trial files
data TrialEntry = Issue Command | Perform Challenge | Comment String deriving Show

type Trial = [TrialEntry]


commandPrefixChar :: Char
commandPrefixChar = '!'
commentPrefixChar :: Char
commentPrefixChar = '#'
challengeSeperatorChar :: Char
challengeSeperatorChar = ':'

scrubPrompt :: Prompt -> Prompt
scrubPrompt = map toUpper

-- a class for writing to files
class Writable a where
    write :: a -> String

instance Writable Command where
    write RandomizeLayoutOrder = "randomize-layouts"
    write NextLayout = "next-layout"
    write (TrialNumber n) = "trial-number " ++ show n

instance Writable Challenge where
    write (Blind p) = "blind" ++ [challengeSeperatorChar] ++ scrubPrompt p
    write (Perfect p) = "perfect" ++ [challengeSeperatorChar] ++ scrubPrompt p
    write (Practice p) = "practice" ++ [challengeSeperatorChar] ++ scrubPrompt p

instance Writable TrialEntry where
    write (Issue c) = commandPrefixChar : write c
    write (Perform c) = write c
    write (Comment c) = commentPrefixChar : c

instance (Writable a) => Writable [a] where
    write = intercalate "\n" . map write

perform :: (Prompt -> Challenge) -> Prompt -> TrialEntry
perform t s = Perform (t s)

type Main = IO ()

labelTrial :: Trial -> Int -> Trial
labelTrial trial n = (Issue $ TrialNumber n) : trial

writeTrial :: Int -> Trial -> Main
writeTrial n trial = writeFile ("Assets/Trials/trial" ++ show n ++ ".txt") (write trial)

writeTrials :: [Main] -> Main
writeTrials [] = putStr "done."
writeTrials (first:rest) =
    do
        first
        writeTrials rest

main :: Main
main = writeTrials $ map writer $ zip trials [0..]
    where writer (t, n) = writeTrial n $ labelTrial t n


-- TRIALS

trials :: [Trial]
trials = 
    [ trialA
    , trialB 
    ] 

trialA :: Trial
trialA =
    [ Comment "arbitrarily made by logan"
    , Issue RandomizeLayoutOrder
    , perform Blind "the dog took a leap"
    , Issue NextLayout
    , perform Perfect "and hit the ground softly"
    ]

trialB :: Trial
trialB =
    [ Comment "arbitrarily made by logan"
    , Issue RandomizeLayoutOrder
    , perform Blind "abcde"
    , perform Blind "123456"
    ]