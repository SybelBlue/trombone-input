{-# LANGUAGE LambdaCase #-}

import Data.List

type Layout = [LayoutKey]

data LayoutKey = Simple Char Size | Ambiguous [ LayoutKey ] | Alt Char Size (Maybe Char) deriving Show

data Size = Small | Medium deriving Show

sizeToBarWidth :: Size -> Int
sizeToBarWidth =
    \case
      Small -> 2
      Medium -> 3

qwerty :: Layout
qwerty = map tripletIntoAmbiguous $ [ "QAZ", "WSX", "EDC", "RFV", "TGB"] ++ map reverse ["YHU", "IJN", "OKM", "PL." ]

tripletIntoAmbiguous :: String -> LayoutKey
tripletIntoAmbiguous [ a, b, c ] = Ambiguous [ Simple a Small, Simple b Medium, Simple c Small ]
tripletIntoAmbiguous _ = error "unexpected input"

layoutSize :: Layout -> Int
layoutSize = sum . map itemSize

itemSize :: LayoutKey -> Int
itemSize (Simple _ s) = sizeToBarWidth s
itemSize (Ambiguous xs) = layoutSize xs
itemSize _ = sizeToBarWidth Small

abcde :: Layout
abcde = map makeItem ['A'..'Z']
  where makeItem x = Simple x $ if commonLetter x then Medium else Small

binnedAbcde :: Layout
binnedAbcde = map Ambiguous . splitEvery 4 . map makeItem $ zip ['A'..'Z'] alt
  where 
    makeItem (c, a) = Alt c Small a
    alt = concat . transpose $
          [ [ Just '1', Just '2', Just '3', Nothing, Nothing, Nothing, Nothing ]
          , [ Just '4', Just '5', Just '6', Nothing, Nothing, Nothing, Nothing ]
          , [ Just '7', Just '8', Just '9', Nothing, Nothing, Nothing ]
          , [ Just '*', Just '/', Just '.', Nothing, Nothing, Nothing ] 
          ]

-- https://stackoverflow.com/questions/8680888/subdividing-a-list-in-haskell
splitEvery :: Int -> [a] -> [[a]]
splitEvery _ [] = []
splitEvery n list = first : (splitEvery n rest)
  where
    (first,rest) = splitAt n list

commonLetter :: Char -> Bool
commonLetter x = elem x "ERIOTAN"

makeInitMethod :: Layout -> String
makeInitMethod layout = "\t// Auto-generated \n\tprotected override LayoutKey[] FillKeys() {\n\t\t"
  ++ "return new LayoutKey[] {\n"
  ++ (intercalate "\n" $ concat $ map (map deformat . makeConstructorLineFor) layout)
  ++ "\n\t};\n}"


type Formatted = (Int, String, Bool)

deformat :: Formatted -> String
deformat (n, s, b) = (take n $ cycle "\t") ++ s ++ if b then "," else ""

makeConstructorLineFor :: LayoutKey -> [Formatted]
makeConstructorLineFor (Simple c s) = [(2, "new SimpleKey('" ++ [c, '\'', ',', ' '] ++ show (sizeToBarWidth s) ++ ")", True)]
makeConstructorLineFor (Alt c s Nothing) = [(2, "new SimpleKey('" ++ [c, '\'', ',', ' '] ++ show (sizeToBarWidth s) ++ ")", True)]
makeConstructorLineFor (Alt c s (Just a)) = [(2, "new SimpleKey('" ++ [c, '\'', ',', ' '] ++ show (sizeToBarWidth s) ++ ", " ++ show a ++  ")", True)]
makeConstructorLineFor (Ambiguous items) = (2, "new AmbiguousKey(true", True) : inner ++ [(2, ")", True)]
  where 
    inner =
      case uncons . reverse . map (\(n, s, b) -> (n + 1, s, b)) . concat $ map makeConstructorLineFor items of
        Just ((n, s, _), others) ->
          reverse $ (n, s, False) : others
        Nothing ->
          []
  

main = putStr $ makeInitMethod binnedAbcde