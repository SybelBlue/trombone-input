{-# LANGUAGE LambdaCase #-}

import Data.List (intercalate)

type Layout = [LayoutKey]

data LayoutKey = Newline | Backspace | Simple Char Size | Ambiguous [ LayoutKey ] deriving Show

data Size = Small | Medium deriving Show

sizeToBarWidth :: Size -> Int
sizeToBarWidth =
    \case
      Small -> 2
      Medium -> 3

qwerty :: Layout
qwerty = map tripletIntoAmbiguous $ [ "QAZ", "WSX", "EDC", "RFV", "TGB"] ++ map reverse ["YHU", "IJN", "OKM", "PL." ]

cmdRow :: LayoutKey
cmdRow = 
    Ambiguous [ Backspace, Simple ' ' Medium, Newline ]

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
binnedAbcde = map makeItem $ splitEvery 4 ['A'..'Z']
  where makeItem = Ambiguous . map (\c -> Simple c Small)

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
  ++ "\n\t\treturn new LayoutKey[] {\n"
  ++ (intercalate ",\n" $ foldl (++) [] $ map (map deformat . makeConstructorLineFor) layout)
  ++ "\n\t\t\t};\n\t}"


type Formatted = (Int, String)

deformat :: Formatted -> String
deformat (n, s) = (take n $ cycle "\t") ++ s

makeConstructorLineFor :: LayoutKey -> [Formatted]
makeConstructorLineFor (Simple c s) = [(4, "new SimpleKey('" ++ [c, '\'', ',', ' '] ++ show (sizeToBarWidth s) ++ ")")]
makeConstructorLineFor (Ambiguous items) = (4, "new AmbiguousKey(true") : inner ++ [(4, ")")]
  where inner = map (\(n, s) -> (n + 1, s)) $ foldl (++) [] $ map makeConstructorLineFor items
  

main = putStr $ makeInitMethod binnedAbcde