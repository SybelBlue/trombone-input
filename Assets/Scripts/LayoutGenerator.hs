{-# LANGUAGE LambdaCase #-}

import Data.List (intercalate)

type Layout = [LayoutItem]

data LayoutItem = Newline | Backspace | Item Char Size | Block [ LayoutItem ] deriving Show

data Size = Small | Medium deriving Show

sizeToBarWidth :: Size -> Int
sizeToBarWidth =
    \case
      Small -> 2
      Medium -> 3

qwerty :: Layout
qwerty = map tripletIntoBlock $ [ "QAZ", "WSX", "EDC", "RFV", "TGB"] ++ map reverse ["YHU", "IJN", "OKM", "PL." ]

cmdRow :: LayoutItem
cmdRow = 
    Block [ Backspace, Item ' ' Medium, Newline ]

tripletIntoBlock :: String -> LayoutItem
tripletIntoBlock [ a, b, c ] = Block [ Item a Small, Item b Medium, Item c Small ]
tripletIntoBlock _ = error "unexpected input"

layoutSize :: Layout -> Int
layoutSize = sum . map itemSize

itemSize :: LayoutItem -> Int
itemSize (Item _ s) = sizeToBarWidth s
itemSize (Block xs) = layoutSize xs
itemSize _ = sizeToBarWidth Small

abcde :: Layout
abcde = map makeItem ['A'..'Z']
  where makeItem x = Item x $ if commonLetter x then Medium else Small

commonLetter :: Char -> Bool
commonLetter x = elem x "ERIOTAN"

makeInitMethod :: Layout -> String
makeInitMethod layout = "\t// Auto-generated \n\tprivate override LayoutKey[] FillKeys() {\n\t\t"
  ++ (intercalate "\n\t\t" $ drop 1 lines)
  ++ "\n\t\treturn new LayoutKey[] {\n\t\t\t"
  ++ intercalate ",\n\t\t\t" names
  ++ "\n\t\t};\n\t}\n"
  where (_, lines, names) = foldl itemConstructorFolder (0, [], []) layout

itemConstructorFolder :: (Int, [ String ], [ String ]) -> LayoutItem -> (Int, [ String ], [ String ])
itemConstructorFolder (prev, prevItems, prevNames) item =
    (next, prevItems ++ ["\n"] ++ nextItems, prevNames ++ nextNames)
    where (next, nextItems, nextNames) = makeItemConstructor (prev, item)

makeItemConstructor :: (Int, LayoutItem) -> (Int, [ String ], [ String ])
makeItemConstructor (n, (Item c s)) =
    ( n + 1
    , [ "var " ++ itemName ++ " = ScriptableObject.CreateInstance<SimpleKey>();"
      , itemName ++ ".init('" ++ [c] ++ "', " ++ show (sizeToBarWidth s) ++ ");"
      ]
    , [ itemName ]
    )
    where itemName = "basicItem" ++ show n
makeItemConstructor (n, (Block xs)) =
  ( next + 1
  , previous ++ 
    [ "var " ++ itemName ++ " = ScriptableObject.CreateInstance<AmbiguousKey>();"
    , itemName ++ ".init(" ++ (intercalate ", " (slant : names)) ++ ");"
    ]
  , [ itemName ]
  )
  where 
      slant = if (next < 20) then "true" else "false"
      itemName = "blockItem" ++ show next
      (next, previous, names) = foldl foldFn (n, [], []) xs
      foldFn (pn, plines, pnames) item = 
          (innern, plines ++ innerlines, pnames ++ innernames)
          where (innern, innerlines, innernames) = makeItemConstructor (pn, item)

main = putStr $ makeInitMethod abcde